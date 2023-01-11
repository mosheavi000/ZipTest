using Ionic.Zip;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace NewSiteServer.Services
{
    public class HandlerService : IHandlerService
    {
        const string DIR = @"C:\Users\ad\Documents\";
        const string CANDIDATES_DIRECTORY = $"{DIR}Candidate\\";
        const string OUTPUT_DIRECTORY = $"{DIR}Output\\";

        public HandlerService()
        {
            if (!Directory.Exists(CANDIDATES_DIRECTORY))
            {
                Directory.CreateDirectory(CANDIDATES_DIRECTORY);
            }
            if (!Directory.Exists(OUTPUT_DIRECTORY))
            {
                Directory.CreateDirectory(OUTPUT_DIRECTORY);
            }
        }
        public object CreateRSAKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                var privateKey = rsa.ExportParameters(true);
                var publicKey = rsa.ExportParameters(false);
                CreateXmlFileKeys(privateKey, DIR + "private_key.xml");
                CreateXmlFileKeys(publicKey, DIR + "public_key.xml");
                var result = new
                {
                    PrivateKey = DIR + "private_key.xml",
                    PublicKey = DIR + "public_key.xml"
                };
                return result;
            }
        }

        public async Task<string> SubmitMyPosition(Candidate candidate)
        {
            if (candidate != null && candidate.IDNumber != null && candidate.Attachment != null)
            {
                var candidateDirectory = CANDIDATES_DIRECTORY + candidate.IDNumber;
                if (Directory.Exists(candidateDirectory))
                {
                    Directory.Delete(candidateDirectory, true);
                }
                Directory.CreateDirectory(candidateDirectory);
                await DownloadFile(candidateDirectory, candidate.Attachment);
                CreateCandidateXmlFile(candidateDirectory, candidate);
                var password = GeneratePassword();
                CompressedFiles(candidateDirectory, candidate.IDNumber, password);
                var encryptionPassword = Encryption(password);
                File.WriteAllText(candidateDirectory + "\\key.txt", encryptionPassword);
                SendFiles(candidateDirectory, candidate.IDNumber);
                return "Upload successfully";
            }
            else
            {
                throw new Exception("Invalid input");
            }

        }

        private void SendFiles(string candidateDirectory, string IDNumber)
        {
            var sendFilesDir = candidateDirectory + "\\SendFiles";
            Directory.CreateDirectory(sendFilesDir);
            string[] files = Directory.GetFiles(candidateDirectory);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Name == "key.txt" || fi.Name == "candidate.zip")
                {
                    File.Move(file, sendFilesDir + "\\" + fi.Name);
                }
            }
            CompressedFiles(sendFilesDir, IDNumber);
            var candidateZipName = $"{OUTPUT_DIRECTORY}\\{IDNumber}.zip";
            if (File.Exists(candidateZipName))
            {
                File.Delete(candidateZipName);
            }
            File.Move($"{sendFilesDir}\\{IDNumber}.zip", candidateZipName);
        }

        private async Task DownloadFile(string candidateDirectory, IFormFile file)
        {
            string filePath = Path.Combine(candidateDirectory, file.FileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        private void CreateCandidateXmlFile(string candidateDirectory, Candidate candidate)
        {
     
            var filename = $"{candidateDirectory}\\{candidate.IDNumber}.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(Candidate));
            using (TextWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, candidate);
            }
        }

        private void CreateXmlFileKeys(RSAParameters key, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RSAParameters));
            using (TextWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, key);
            }
        }

        private void CompressedFiles(string directory, string IDNumber, string? password = null)
        {
            var zipFile = password != null ? $"{directory}\\candidate.zip" : $"{directory}\\{IDNumber}.zip";
            using (ZipFile zip = new ZipFile())
            {
                if (password != null)
                {
                    zip.Password = password;
                }
                zip.AddDirectory(directory);
                zip.Save(zipFile);
            }
        }

        private string GeneratePassword()
        {
            AesCryptoServiceProvider crypto = new AesCryptoServiceProvider();
            crypto.KeySize = 256;
            crypto.BlockSize = 128;
            crypto.GenerateKey();
            byte[] keyGenerated = crypto.Key;
            return Convert.ToBase64String(keyGenerated);
        }
        private string Encryption(string strText)
        {

            string publicKey = File.ReadAllText(DIR + "public_key.xml");
            var testData = Encoding.UTF8.GetBytes(strText);

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    rsa.FromXmlString(publicKey.ToString());
                    var encryptedData = rsa.Encrypt(testData, true);
                    var base64Encrypted = Convert.ToBase64String(encryptedData);
                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }


    }
}
