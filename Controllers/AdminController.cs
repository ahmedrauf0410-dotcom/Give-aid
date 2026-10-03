using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GiveAID.Data;
using GiveAID.Models;
using GiveAID.Services;
using GiveAID.ViewModels;

namespace GiveAID.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly ISiteSettingService _siteSettingService;

        public AdminController(
            ApplicationDbContext context,
            IFileUploadService fileUploadService,
            ISiteSettingService siteSettingService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _siteSettingService = siteSettingService;
        }

        // GET: /Admin or /Admin/Dashboard
        [Route("Admin")]
        [Route("Admin/Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalDonations = await _context.Donations.CountAsync(d => d.PaymentStatus == "Successful");
            var totalDonationAmount = await _context.Donations
                .Where(d => d.PaymentStatus == "Successful")
                .SumAsync(d => (decimal?)d.Amount) ?? 0.00m;

            var totalCauses = await _context.DonationCauses.CountAsync();
            var totalProgrammes = await _context.Programmes.CountAsync();
            var totalPartners = await _context.Partners.CountAsync();
            var totalGallery = await _context.GalleryItems.CountAsync();
            var totalQueries = await _context.UserQueries.CountAsync();
            var totalPendingQueries = await _context.UserQueries.CountAsync(q => q.Status == "Pending");
            var totalInterests = await _context.ProgrammeInterests.CountAsync();
            var totalContacts = await _context.ContactMessages.CountAsync();
            var totalInvitations = await _context.Invitations.CountAsync();

            var recentDonations = await _context.Donations
                .Include(d => d.DonationCause)
                .OrderByDescending(d => d.DonationDate)
                .Take(5)
                .ToListAsync();

            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentQueries = await _context.UserQueries
                .OrderByDescending(q => q.CreatedAt)
                .Take(5)
                .ToListAsync();

            var topCauses = await _context.DonationCauses
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.RaisedAmount)
                .Take(5)
                .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalDonations = totalDonations,
                TotalDonationAmount = totalDonationAmount,
                TotalCauses = totalCauses,
                TotalProgrammes = totalProgrammes,
                TotalPartners = totalPartners,
                TotalGalleryImages = totalGallery,
                TotalQueries = totalQueries,
                TotalPendingQueries = totalPendingQueries,
                TotalProgrammeInterests = totalInterests,
                TotalContactMessages = totalContacts,
                TotalInvitations = totalInvitations,
                RecentDonations = recentDonations,
                RecentUsers = recentUsers,
                RecentQueries = recentQueries,
                TopCauses = topCauses
            };

            return View(model);
        }

        // ==========================================
        // 1. DONATION CAUSES CRUD
        // ==========================================

        public async Task<IActionResult> Causes()
        {
            var causes = await _context.DonationCauses
                .Include(c => c.Donations)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(causes);
        }

        public IActionResult CreateCause()
        {
            return View(new CauseEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCause(CauseEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? imagePath = null;
            if (model.ImageFile != null)
            {
                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(model.ImageFile, "causes");
                if (!success)
                {
                    ModelState.AddModelError("ImageFile", error ?? "Image upload failed.");
                    return View(model);
                }
                imagePath = filePath;
            }

            var cause = new DonationCause
            {
                Name = model.Name.Trim(),
                Category = model.Category.Trim(),
                ShortDescription = model.ShortDescription.Trim(),
                FullDescription = model.FullDescription.Trim(),
                TargetAmount = model.TargetAmount,
                RaisedAmount = model.RaisedAmount,
                ImageUrl = imagePath ?? "/images/cause-children.jpg",
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.DonationCauses.Add(cause);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Donation cause '{cause.Name}' created successfully.";
            return RedirectToAction(nameof(Causes));
        }

        public async Task<IActionResult> EditCause(int id)
        {
            var cause = await _context.DonationCauses.FindAsync(id);
            if (cause == null) return NotFound();

            var model = new CauseEditViewModel
            {
                Id = cause.Id,
                Name = cause.Name,
                Category = cause.Category,
                ShortDescription = cause.ShortDescription,
                FullDescription = cause.FullDescription,
                TargetAmount = cause.TargetAmount,
                RaisedAmount = cause.RaisedAmount,
                ExistingImageUrl = cause.ImageUrl,
                IsActive = cause.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCause(CauseEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var cause = await _context.DonationCauses.FindAsync(model.Id);
            if (cause == null) return NotFound();

            if (model.ImageFile != null)
            {
                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(model.ImageFile, "causes");
                if (!success)
                {
                    ModelState.AddModelError("ImageFile", error ?? "Image upload failed.");
                    return View(model);
                }
                _fileUploadService.DeleteFile(cause.ImageUrl);
                cause.ImageUrl = filePath;
            }

            cause.Name = model.Name.Trim();
            cause.Category = model.Category.Trim();
            cause.ShortDescription = model.ShortDescription.Trim();
            cause.FullDescription = model.FullDescription.Trim();
            cause.TargetAmount = model.TargetAmount;
            cause.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cause '{cause.Name}' updated successfully.";
            return RedirectToAction(nameof(Causes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCauseStatus(int id)
        {
            var cause = await _context.DonationCauses.FindAsync(id);
            if (cause != null)
            {
                cause.IsActive = !cause.IsActive;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cause '{cause.Name}' status updated to {(cause.IsActive ? "Active" : "Inactive")}.";
            }
            return RedirectToAction(nameof(Causes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCause(int id)
        {
            var cause = await _context.DonationCauses
                .Include(c => c.Donations)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cause == null) return NotFound();

            if (cause.Donations.Any())
            {
                // Soft-delete to protect audit trail
                cause.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["InfoMessage"] = $"Cause '{cause.Name}' contains historical donations and was deactivated rather than hard-deleted.";
            }
            else
            {
                _fileUploadService.DeleteFile(cause.ImageUrl);
                _context.DonationCauses.Remove(cause);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cause '{cause.Name}' was permanently removed.";
            }

            return RedirectToAction(nameof(Causes));
        }

        // ==========================================
        // 2. PROGRAMMES CRUD & INTERESTS
        // ==========================================

        public async Task<IActionResult> Programmes()
        {
            var programmes = await _context.Programmes
                .Include(p => p.ProgrammeInterests)
                .OrderByDescending(p => p.EventDate)
                .ToListAsync();
            return View(programmes);
        }

        public IActionResult CreateProgramme()
        {
            return View(new ProgrammeEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProgramme(ProgrammeEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? imagePath = null;
            if (model.ImageFile != null)
            {
                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(model.ImageFile, "programmes");
                if (!success)
                {
                    ModelState.AddModelError("ImageFile", error ?? "Image upload failed.");
                    return View(model);
                }
                imagePath = filePath;
            }

            var programme = new Programme
            {
                Title = model.Title.Trim(),
                Category = model.Category.Trim(),
                ShortDescription = model.ShortDescription.Trim(),
                FullDescription = model.FullDescription.Trim(),
                EventDate = model.EventDate,
                Location = model.Location.Trim(),
                Status = model.Status,
                ImageUrl = imagePath ?? "/images/prog-winter.jpg",
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Programmes.Add(programme);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Programme '{programme.Title}' created successfully.";
            return RedirectToAction(nameof(Programmes));
        }

        public async Task<IActionResult> EditProgramme(int id)
        {
            var programme = await _context.Programmes.FindAsync(id);
            if (programme == null) return NotFound();

            var model = new ProgrammeEditViewModel
            {
                Id = programme.Id,
                Title = programme.Title,
                Category = programme.Category,
                ShortDescription = programme.ShortDescription,
                FullDescription = programme.FullDescription,
                EventDate = programme.EventDate,
                Location = programme.Location,
                Status = programme.Status,
                ExistingImageUrl = programme.ImageUrl,
                IsActive = programme.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProgramme(ProgrammeEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var programme = await _context.Programmes.FindAsync(model.Id);
            if (programme == null) return NotFound();

            if (model.ImageFile != null)
            {
                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(model.ImageFile, "programmes");
                if (!success)
                {
                    ModelState.AddModelError("ImageFile", error ?? "Image upload failed.");
                    return View(model);
                }
                _fileUploadService.DeleteFile(programme.ImageUrl);
                programme.ImageUrl = filePath;
            }

            programme.Title = model.Title.Trim();
            programme.Category = model.Category.Trim();
            programme.ShortDescription = model.ShortDescription.Trim();
            programme.FullDescription = model.FullDescription.Trim();
            programme.EventDate = model.EventDate;
            programme.Location = model.Location.Trim();
            programme.Status = model.Status;
            programme.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Programme '{programme.Title}' updated successfully.";
            return RedirectToAction(nameof(Programmes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProgramme(int id)
        {
            var programme = await _context.Programmes.FindAsync(id);
            if (programme == null) return NotFound();

            _fileUploadService.DeleteFile(programme.ImageUrl);
            _context.Programmes.Remove(programme);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Programme '{programme.Title}' removed.";
            return RedirectToAction(nameof(Programmes));
        }

        public async Task<IActionResult> ProgrammeInterests(int? programmeId)
        {
            var query = _context.ProgrammeInterests.Include(i => i.Programme).AsQueryable();

            if (programmeId.HasValue && programmeId.Value > 0)
            {
                query = query.Where(i => i.ProgrammeId == programmeId.Value);
                ViewBag.FilteredProgramme = await _context.Programmes.FindAsync(programmeId.Value);
            }

            var list = await query.OrderByDescending(i => i.CreatedAt).ToListAsync();
            return View(list);
        }

        // ==========================================
        // 3. PARTNERS CRUD
        // ==========================================

        public async Task<IActionResult> Partners()
        {
            var partners = await _context.Partners
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            return View(partners);
        }

        public IActionResult CreatePartner()
        {
            return View(new PartnerEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartner(PartnerEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? logoPath = null;
            if (model.LogoFile != null)
            {
                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(model.LogoFile, "partners");
                if (!success)
                {
                    ModelState.AddModelError("LogoFile", error ?? "Logo upload failed.");
                    return View(model);
                }
                logoPath = filePath;
            }

            var partner = new Partner
            {
                Name = model.Name.Trim(),
                Category = model.Category.Trim(),
                Description = model.Description.Trim(),
                WebsiteUrl = model.WebsiteUrl?.Trim(),
                LogoUrl = logoPath ?? "/images/partner-gho.png",
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Partners.Add(partner);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Partner '{partner.Name}' added successfully.";
            return RedirectToAction(nameof(Partners));
        }

        public async Task<IActionResult> EditPartner(int id)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner == null) return NotFound();

            var model = new PartnerEditViewModel
            {
                Id = partner.Id,
                Name = partner.Name,
                Category = partner.Category,
                Description = partner.Description,
                WebsiteUrl = partner.WebsiteUrl,
                DisplayOrder = partner.DisplayOrder,
                ExistingLogoUrl = partner.LogoUrl,
                IsActive = partner.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartner(PartnerEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var partner = await _context.Partners.FindAsync(model.Id);
            if (partner == null) return NotFound();

            if (model.LogoFile != null)
            {
                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(model.LogoFile, "partners");
                if (!success)
                {
                    ModelState.AddModelError("LogoFile", error ?? "Logo upload failed.");
                    return View(model);
                }
                _fileUploadService.DeleteFile(partner.LogoUrl);
                partner.LogoUrl = filePath;
            }

            partner.Name = model.Name.Trim();
            partner.Category = model.Category.Trim();
            partner.Description = model.Description.Trim();
            partner.WebsiteUrl = model.WebsiteUrl?.Trim();
            partner.DisplayOrder = model.DisplayOrder;
            partner.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Partner '{partner.Name}' updated successfully.";
            return RedirectToAction(nameof(Partners));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePartner(int id)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner == null) return NotFound();

            _fileUploadService.DeleteFile(partner.LogoUrl);
            _context.Partners.Remove(partner);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Partner '{partner.Name}' was deleted.";
            return RedirectToAction(nameof(Partners));
        }

        // ==========================================
        // 4. ABOUT SECTIONS CMS (CRITICAL REQUIREMENT)
        // ==========================================

        public async Task<IActionResult> AboutSections()
        {
            var sections = await _context.AboutSections
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
            return View(sections);
        }

        public async Task<IActionResult> EditAboutSection(int id)
        {
            var section = await _context.AboutSections.FindAsync(id);
            if (section == null) return NotFound();

            var model = new AboutSectionEditViewModel
            {
                Id = section.Id,
                SectionKey = section.SectionKey,
                Title = section.Title,
                Subtitle = section.Subtitle,
                Content = section.Content,
                DisplayOrder = section.DisplayOrder,
                ExistingImageUrl = section.ImageUrl,
                IsActive = section.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAboutSection(AboutSectionEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var section = await _context.AboutSections.FindAsync(model.Id);
            if (section == null) return NotFound();

            if (model.ImageFile != null)
            {
                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(model.ImageFile, "about");
                if (!success)
                {
                    ModelState.AddModelError("ImageFile", error ?? "Image upload failed.");
                    return View(model);
                }
                _fileUploadService.DeleteFile(section.ImageUrl);
                section.ImageUrl = filePath;
            }

            section.Title = model.Title.Trim();
            section.Subtitle = model.Subtitle?.Trim();
            section.Content = model.Content.Trim();
            section.DisplayOrder = model.DisplayOrder;
            section.IsActive = model.IsActive;
            section.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"About Section '{section.Title}' updated! Changes are immediately live on /About.";
            return RedirectToAction(nameof(AboutSections));
        }

        // ==========================================
        // 5. GALLERY CRUD
        // ==========================================

        public async Task<IActionResult> Gallery()
        {
            var items = await _context.GalleryItems
                .OrderBy(g => g.DisplayOrder)
                .ToListAsync();
            return View(items);
        }

        public IActionResult CreateGalleryItem()
        {
            return View(new GalleryItemEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGalleryItem(GalleryItemEditViewModel model)
        {
            if (model.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Please upload an image file.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, filePath, error) = await _fileUploadService.UploadFileAsync(model.ImageFile!, "gallery");
            if (!success)
            {
                ModelState.AddModelError("ImageFile", error ?? "Failed to upload image.");
                return View(model);
            }

            var item = new GalleryItem
            {
                Title = model.Title.Trim(),
                Category = model.Category.Trim(),
                Description = model.Description?.Trim(),
                ImageUrl = filePath!,
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.GalleryItems.Add(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Gallery image '{item.Title}' added successfully.";
            return RedirectToAction(nameof(Gallery));
        }

        public async Task<IActionResult> EditGalleryItem(int id)
        {
            var item = await _context.GalleryItems.FindAsync(id);
            if (item == null) return NotFound();

            var model = new GalleryItemEditViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Category = item.Category,
                Description = item.Description,
                DisplayOrder = item.DisplayOrder,
                ExistingImageUrl = item.ImageUrl,
                IsActive = item.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGalleryItem(GalleryItemEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var item = await _context.GalleryItems.FindAsync(model.Id);
            if (item == null) return NotFound();

            if (model.ImageFile != null)
            {
                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(model.ImageFile, "gallery");
                if (!success)
                {
                    ModelState.AddModelError("ImageFile", error ?? "Upload failed.");
                    return View(model);
                }
                _fileUploadService.DeleteFile(item.ImageUrl);
                item.ImageUrl = filePath!;
            }

            item.Title = model.Title.Trim();
            item.Category = model.Category.Trim();
            item.Description = model.Description?.Trim();
            item.DisplayOrder = model.DisplayOrder;
            item.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Gallery item '{item.Title}' updated.";
            return RedirectToAction(nameof(Gallery));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGalleryItem(int id)
        {
            var item = await _context.GalleryItems.FindAsync(id);
            if (item == null) return NotFound();

            _fileUploadService.DeleteFile(item.ImageUrl);
            _context.GalleryItems.Remove(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Gallery item '{item.Title}' removed.";
            return RedirectToAction(nameof(Gallery));
        }

        // ==========================================
        // 6. HELP CENTRE / FAQS CRUD
        // ==========================================

        public async Task<IActionResult> HelpCentre()
        {
            var faqs = await _context.HelpItems
                .OrderBy(h => h.DisplayOrder)
                .ToListAsync();
            return View(faqs);
        }

        public IActionResult CreateHelpItem()
        {
            return View(new HelpItemEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHelpItem(HelpItemEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var item = new HelpItem
            {
                Question = model.Question.Trim(),
                Answer = model.Answer.Trim(),
                Category = model.Category.Trim(),
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.HelpItems.Add(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "FAQ item created.";
            return RedirectToAction(nameof(HelpCentre));
        }

        public async Task<IActionResult> EditHelpItem(int id)
        {
            var item = await _context.HelpItems.FindAsync(id);
            if (item == null) return NotFound();

            var model = new HelpItemEditViewModel
            {
                Id = item.Id,
                Question = item.Question,
                Answer = item.Answer,
                Category = item.Category,
                DisplayOrder = item.DisplayOrder,
                IsActive = item.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHelpItem(HelpItemEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var item = await _context.HelpItems.FindAsync(model.Id);
            if (item == null) return NotFound();

            item.Question = model.Question.Trim();
            item.Answer = model.Answer.Trim();
            item.Category = model.Category.Trim();
            item.DisplayOrder = model.DisplayOrder;
            item.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "FAQ item updated.";
            return RedirectToAction(nameof(HelpCentre));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHelpItem(int id)
        {
            var item = await _context.HelpItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.HelpItems.Remove(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "FAQ item deleted.";
            return RedirectToAction(nameof(HelpCentre));
        }

        // ==========================================
        // 7. DONATIONS AUDIT & DETAILS
        // ==========================================

        public async Task<IActionResult> Donations(int? causeId, string? status)
        {
            var query = _context.Donations.Include(d => d.DonationCause).Include(d => d.User).AsQueryable();

            if (causeId.HasValue && causeId.Value > 0)
            {
                query = query.Where(d => d.DonationCauseId == causeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(d => d.PaymentStatus == status);
            }

            var donations = await query.OrderByDescending(d => d.DonationDate).ToListAsync();

            ViewBag.Causes = await _context.DonationCauses.OrderBy(c => c.Name).ToListAsync();
            ViewBag.SelectedCauseId = causeId;
            ViewBag.SelectedStatus = status ?? "All";
            ViewBag.TotalFilteredAmount = donations.Where(d => d.PaymentStatus == "Successful").Sum(d => d.Amount);

            return View(donations);
        }

        public async Task<IActionResult> DonationDetails(int id)
        {
            var donation = await _context.Donations
                .Include(d => d.DonationCause)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donation == null) return NotFound();
            return View(donation);
        }

        // ==========================================
        // 8. REGISTERED USERS MANAGEMENT
        // ==========================================

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Include(u => u.Donations)
                .Include(u => u.Queries)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _context.Users
                .Include(u => u.Donations).ThenInclude(d => d.DonationCause)
                .Include(u => u.Queries)
                .Include(u => u.ProgrammeInterests).ThenInclude(i => i.Programme)
                .Include(u => u.Invitations)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"User '{user.FullName}' status changed to {(user.IsActive ? "Active" : "Inactive")}.";
            }
            return RedirectToAction(nameof(Users));
        }

        // ==========================================
        // 9. QUERIES & SUPPORT
        // ==========================================

        public async Task<IActionResult> Queries(string? status)
        {
            var query = _context.UserQueries.Include(q => q.User).AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(q => q.Status == status);
            }

            var queries = await query.OrderByDescending(q => q.CreatedAt).ToListAsync();
            ViewBag.SelectedStatus = status ?? "All";
            return View(queries);
        }

        public async Task<IActionResult> QueryDetails(int id)
        {
            var query = await _context.UserQueries
                .Include(q => q.User)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (query == null) return NotFound();

            var model = new QueryReplyViewModel
            {
                Id = query.Id,
                SenderName = query.SenderName,
                SenderEmail = query.SenderEmail,
                Subject = query.Subject,
                Message = query.Message,
                CreatedAt = query.CreatedAt,
                Status = query.Status,
                AdminResponse = query.AdminResponse ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QueryDetails(QueryReplyViewModel model)
        {
            var query = await _context.UserQueries.FindAsync(model.Id);
            if (query == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            query.Status = model.Status;
            query.AdminResponse = model.AdminResponse.Trim();
            query.RespondedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Query #{query.Id} updated with response and status '{query.Status}'.";
            return RedirectToAction(nameof(Queries));
        }

        // ==========================================
        // 10. CONTACT MESSAGES
        // ==========================================

        public async Task<IActionResult> Contacts()
        {
            var messages = await _context.ContactMessages
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
            return View(messages);
        }

        public async Task<IActionResult> ContactDetails(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null) return NotFound();

            if (!message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null)
            {
                _context.ContactMessages.Remove(message);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contact inquiry deleted.";
            }
            return RedirectToAction(nameof(Contacts));
        }

        // ==========================================
        // 11. INVITATIONS LOGS
        // ==========================================

        public async Task<IActionResult> Invitations()
        {
            var invitations = await _context.Invitations
                .Include(i => i.SenderUser)
                .OrderByDescending(i => i.SentAt)
                .ToListAsync();
            return View(invitations);
        }

        // ==========================================
        // 12. SITE SETTINGS
        // ==========================================

        public async Task<IActionResult> SiteSettings()
        {
            var settings = await _siteSettingService.GetSettingsAsync();
            var model = new SiteSettingEditViewModel
            {
                Id = settings.Id,
                NgoName = settings.NgoName,
                Tagline = settings.Tagline,
                Email = settings.Email,
                Phone = settings.Phone,
                Address = settings.Address,
                CityStateZip = settings.CityStateZip,
                FacebookUrl = settings.FacebookUrl,
                TwitterUrl = settings.TwitterUrl,
                InstagramUrl = settings.InstagramUrl,
                YouTubeUrl = settings.YouTubeUrl,
                FooterText = settings.FooterText,
                ExistingLogoUrl = settings.LogoUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SiteSettings(SiteSettingEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? logoPath = model.ExistingLogoUrl;
            if (model.LogoFile != null)
            {
                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(model.LogoFile, "site");
                if (!success)
                {
                    ModelState.AddModelError("LogoFile", error ?? "Logo upload failed.");
                    return View(model);
                }
                logoPath = filePath;
            }

            var setting = new SiteSetting
            {
                Id = model.Id,
                NgoName = model.NgoName.Trim(),
                Tagline = model.Tagline?.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                Phone = model.Phone.Trim(),
                Address = model.Address.Trim(),
                CityStateZip = model.CityStateZip.Trim(),
                FacebookUrl = model.FacebookUrl?.Trim(),
                TwitterUrl = model.TwitterUrl?.Trim(),
                InstagramUrl = model.InstagramUrl?.Trim(),
                YouTubeUrl = model.YouTubeUrl?.Trim(),
                FooterText = model.FooterText.Trim(),
                LogoUrl = logoPath
            };

            await _siteSettingService.UpdateSettingsAsync(setting);

            TempData["SuccessMessage"] = "Site settings updated successfully. Changes are now visible across the public website.";
            return RedirectToAction(nameof(SiteSettings));
        }
    }
}
