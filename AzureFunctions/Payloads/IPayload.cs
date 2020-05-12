using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions.Payloads
{
    interface IPayload
    {
        Task<IActionResult> Handle();
    }
}