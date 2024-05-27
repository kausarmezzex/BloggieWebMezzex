using Bloggie.web.Models.Domain;
using Bloggie.web.Models.ViewModel;
using Bloggie.web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Bloggie.web.Controllers
{
    public class AdminTagsController : Controller
    {
        private readonly ITagRepository _tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> AddAsync(AddTagRequest addTag)
        {
            ValidateAddTagRequest(addTag);

            if (!ModelState.IsValid)
                return View(addTag);

            var tag = new Tag
            {
                Name = addTag.Name,
                DisplayName = addTag.DisplayName,
                ImageUrl = addTag.ImageUrl,
            };

            await _tagRepository.AddAsync(tag);
            TempData["success"] = "Tag added successfully.";
            return RedirectToAction("List");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> ListAsync(
            string? searchQuery,
            string? sortBy,
            string? sortDirection,
            int pageSize = 6,
            int pageNumber = 1)
        {
            var totalRecords = await _tagRepository.CountAsync();
            var totalPages = Math.Ceiling((decimal)totalRecords / pageSize);

            if (pageNumber > totalPages)
                pageNumber--;
            if (pageNumber < 1)
                pageNumber++;

            var tags = await _tagRepository.GetAllAsync(searchQuery, sortBy, sortDirection, pageSize, pageNumber);
            ViewBag.SearchQuery = searchQuery;
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            return View(tags);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditAsync(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);
            if (tag == null)
                return NotFound();

            var editTagRequest = new EditTagRequest
            {
                Id = tag.Id,
                Name = tag.Name,
                DisplayName = tag.DisplayName,
                ImageUrl = tag.ImageUrl  
            };

            return View(editTagRequest);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditAsync(EditTagRequest editTagRequest)
        {
            

            var existingTag = await _tagRepository.GetAsync(editTagRequest.Id);
            if (existingTag == null)
            {
                TempData["error"] = "Tag not found.";
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }

            existingTag.Name = editTagRequest.Name;
            existingTag.DisplayName = editTagRequest.DisplayName;
            existingTag.ImageUrl = editTagRequest.ImageUrl;
            await _tagRepository.UpdateAsync(existingTag);

            TempData["success"] = "Tag updated successfully.";
            return RedirectToAction("List");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deletedTag = await _tagRepository.DeleteAsync(id);
            if (deletedTag == null)
            {
                TempData["error"] = "Failed to delete tag.";
                return RedirectToAction("Edit", new { id = id });
            }

            TempData["success"] = "Tag deleted successfully.";
            return RedirectToAction("List");
        }

        private void ValidateAddTagRequest(AddTagRequest request)
        {
            if (request.Name == request.DisplayName)
            {
                ModelState.AddModelError("DisplayName", "Name cannot be the same as DisplayName");
            }
        }
    }
}
