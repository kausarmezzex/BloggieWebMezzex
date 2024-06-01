using Bloggie.web.Models.Domain;
using Bloggie.web.Models.ViewModel;
using Bloggie.web.Repositories;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Bloggie.web.Controllers
{
    public class AdminTagsController : BaseController
    {
        private readonly ITagRepository _tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add() => View();

        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
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
            TempData["success"] = BlogsResource.Added;
            return RedirectToAction("List");
        }

        //[Authorize(Roles = "Admin")]
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

        //[Authorize(Roles = "Admin")]
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

        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditAsync(EditTagRequest editTagRequest)
        {
            

            var existingTag = await _tagRepository.GetAsync(editTagRequest.Id);
            if (existingTag == null)
            {
                TempData["error"] =BlogsResource.tagNotFound;
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }

            existingTag.Name = editTagRequest.Name;
            existingTag.DisplayName = editTagRequest.DisplayName;
            existingTag.ImageUrl = editTagRequest.ImageUrl;
            await _tagRepository.UpdateAsync(existingTag);

            TempData["success"] = BlogsResource.Update;
            return RedirectToAction("List");
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deletedTag = await _tagRepository.DeleteAsync(id);
            if (deletedTag == null)
            {
                TempData["error"] = BlogsResource.FaildDelete;
                return RedirectToAction("Edit", new { id = id });
            }

            TempData["success"] = BlogsResource.Delete;
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
