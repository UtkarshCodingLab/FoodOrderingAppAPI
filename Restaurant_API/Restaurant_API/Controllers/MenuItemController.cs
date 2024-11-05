using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Restaurant_API.Data;
using Restaurant_API.Models;
using Restaurant_API.Models.Dto;
using System;
using System.Net;

namespace Restaurant_API.Controllers
{
    [Route("api/MenuItem")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private ApiResponse _response;

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _response = new ApiResponse();
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
            _response.Result = _db.MenuItems;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetMenuItem")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            MenuItem menuItem = _db.MenuItems.FirstOrDefault(u => u.Id == id);

            if (menuItem == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            _response.Result = menuItem;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        public async Task<ActionResult<MenuItem>> CreateMenuItem([FromForm] MenuItemCreateDTO menuItemCreateDTO)
        {
            if (menuItemCreateDTO.File.Length > 1 * 1024 * 1024)
            {
                ModelState.AddModelError("CustomError", "File Size More than 1 MB");
                return BadRequest();
            }
            else
            {
                string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
                if (menuItemCreateDTO.File == null)
                {
                    ModelState.AddModelError("CustomError", "Please Upload Image");
                    return BadRequest();
                }
                else
                {
                    var contentPath = _webHostEnvironment.ContentRootPath;
                    var path = Path.Combine(contentPath, "images/menuitem");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    // Check the allowed extenstions
                    var ext = Path.GetExtension(menuItemCreateDTO.File.FileName);
                    if (!allowedFileExtentions.Contains(ext))
                    {
                        ModelState.AddModelError("CustomError", "Unsupported Image File");
                        return BadRequest();
                    }
                    else
                    {
                        // generate a unique filename
                        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
                        var fileNameWithPath = Path.Combine(path, fileName);
                        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
                        menuItemCreateDTO.File.CopyTo(stream);
                        string createdImage = fileName;

                        var menuItem = new MenuItem
                        {
                            Name = menuItemCreateDTO.Name,
                            Description = menuItemCreateDTO.Description,
                            SpecialTag = menuItemCreateDTO.SpecialTag,
                            Category = menuItemCreateDTO.Category,
                            Price = menuItemCreateDTO.Price,
                            Image = createdImage,

                        };
                        _db.MenuItems.Add(menuItem);
                        _db.SaveChanges();
                        return CreatedAtRoute("GetMenuItem", new { id = menuItem.Id }, menuItem);
                    }
                }
            }


        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemUpdateDTO)
        {
            if (id != menuItemUpdateDTO.Id && menuItemUpdateDTO == null)
            {
                return BadRequest();
            }

            MenuItem menuItemFromDB = _db.MenuItems.Find(id);
            if (menuItemFromDB == null)
            {
                return NotFound();
            }

            menuItemFromDB.Name = menuItemUpdateDTO.Name;
            menuItemFromDB.Description = menuItemUpdateDTO.Description;
            menuItemFromDB.SpecialTag = menuItemUpdateDTO.SpecialTag;
            menuItemFromDB.Category = menuItemUpdateDTO.Category;
            menuItemFromDB.Price = menuItemUpdateDTO.Price;

            string oldImage = menuItemFromDB.Image;

            if (menuItemUpdateDTO.File != null && menuItemUpdateDTO.File.Length > 0)
            {
                var contentPath = _webHostEnvironment.ContentRootPath;
                var path = Path.Combine(contentPath, $"images/menuitem", oldImage);
                System.IO.File.Delete(path);

                if (menuItemUpdateDTO.File.Length > 1 * 1024 * 1024)
                {
                    ModelState.AddModelError("CustomError", "File Size More than 1 MB");
                    return BadRequest();
                }
                string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
                if (menuItemUpdateDTO.File == null)
                {
                    ModelState.AddModelError("CustomError", "Please Upload Image");
                    return BadRequest();
                }
                else
                {
                    contentPath = _webHostEnvironment.ContentRootPath;
                    path = Path.Combine(contentPath, "images/menuitem");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    // Check the allowed extenstions
                    var ext = Path.GetExtension(menuItemUpdateDTO.File.FileName);
                    if (!allowedFileExtentions.Contains(ext))
                    {
                        ModelState.AddModelError("CustomError", "Unsupported Image File");
                        return BadRequest();
                    }
                    else
                    {
                        // generate a unique filename
                        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
                        var fileNameWithPath = Path.Combine(path, fileName);
                        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
                        menuItemUpdateDTO.File.CopyTo(stream);
                        string createdImage = fileName;

                        menuItemFromDB.Image = createdImage;

                    }

                    _db.MenuItems.Update(menuItemFromDB);
                    _db.SaveChanges();
                }
            }
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            MenuItem menuItemTobeDeleted = _db.MenuItems.Find(id);
            if (menuItemTobeDeleted == null)
            {
                return BadRequest();
            }

            string deletedImage = menuItemTobeDeleted.Image;

            var contentPath = _webHostEnvironment.ContentRootPath;
            var path = Path.Combine(contentPath, $"images/menuitem", deletedImage);
            System.IO.File.Delete(path);

            _db.MenuItems.Remove(menuItemTobeDeleted);
            _db.SaveChanges();

            return NoContent();


        }
    }

}
