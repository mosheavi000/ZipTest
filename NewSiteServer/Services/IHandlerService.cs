using Microsoft.AspNetCore.Mvc;

namespace NewSiteServer.Services
{
    public interface IHandlerService
    {
        object CreateRSAKeys();
        Task<string> SubmitMyPosition(Candidate candidate);
    }
}
