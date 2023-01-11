using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace NewSiteServer
{
    public class Candidate
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string IDNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string MobilePhone { get; set; }
        [Required]
        public string BirthDate { get; set; }
        [Required]
        public int  ReferSourceID { get; set; }
        [Required]
        public List<Language> Languages { get; set; }

        [Required]
        public int DutyTypeID { get; set; }
        
        public string? Remarks { get; set; }
        public AttachedPosition[]? AttachedPositions { get; set; }
        [Required]
        [XmlIgnore]
        public IFormFile Attachment { get; set; }

    }


    public class Language
    {

        public Language()
        {

        }

        public Language(string languageID, int languageLevelID)
        {
            LanguageID = languageID;
            LanguageLevelID = languageLevelID;
        }

        [Required]
        public string LanguageID { get; set; }
        [Required]
        public int LanguageLevelID { get; set; }
    }

    public class AttachedPosition
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
