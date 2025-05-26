// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using API.Data;
// using API.Entities;
// using API.Extensions;
// using API.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace API.Controllers
// {
//     public class PhotoController(UnitOfWork unitOfWork, PhotoService photoService) : BaseApiController
//     {
//         [HttpPost]
//         [Route("api/photos")]
//         public async Task<IActionResult> UploadPhoto([FromForm] IFormFile file, [FromForm] string? tagNames)
//         {
//             if (file == null || file.Length == 0)
//             {
//                 return BadRequest("No file uploaded");
//             }
//             var result = await photoService.AddPhotoAsync(file);
//             if (result.Error != null) return BadRequest(result.Error.Message);

//             var photo = new Photo
//             {
//                 Url = result.SecureUrl.AbsoluteUri,
//                 PublicId = result.PublicId
//             };

        

//         }
//     }
// }