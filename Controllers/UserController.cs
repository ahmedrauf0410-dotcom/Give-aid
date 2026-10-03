using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GiveAID.Data;
using GiveAID.Models;
using GiveAID.ViewModels;
using GiveAID.Services;
using System.Security.Claims;

namespace GiveAID.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISiteSettingService _siteSettingService;

        public UserController(ApplicationDbContext context, ISiteSettingService siteSettingService)
        {
            _context = context;
            _siteSettingService = siteSettingService;
        }

        // GET: / or /User or /User/Index
        public async Task<IActionResult> Index()
        {
            var settings = await _siteSettingService.GetSettingsAsync();
            var causes = await _context.DonationCauses
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.RaisedAmount)
                .Take(6)
                .ToListAsync();

            var programmes = await _context.Programmes
                .Where(p => p.IsActive && p.EventDate >= DateTime.UtcNow.AddDays(-1))
                .OrderBy(p => p.EventDate)
                .Take(3)
                .ToListAsync();

            var partners = await _context.Partners
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .Take(6)
                .ToListAsync();

            var gallery = await _context.GalleryItems
                .Where(g => g.IsActive)
                .OrderBy(g => g.DisplayOrder)
                .Take(6)
                .ToListAsync();

            var aboutSectionsList = await _context.AboutSections
                .Where(a => a.IsActive)
                .ToListAsync();
            var aboutDict = aboutSectionsList.ToDictionary(a => a.SectionKey, a => a);

            var totalDonationsAmount = await _context.Donations
                .Where(d => d.PaymentStatus == "Successful")
                .SumAsync(d => (decimal?)d.Amount) ?? 0.00m;

            var totalDonationsCount = await _context.Donations
                .CountAsync(d => d.PaymentStatus == "Successful");

            var totalProgrammesCount = await _context.Programmes.CountAsync(p => p.IsActive);
            var totalVolunteersCount = await _context.Users.CountAsync(u => u.IsActive);

            var viewModel = new HomeViewModel
            {
                SiteSetting = settings,
                FeaturedCauses = causes,
                UpcomingProgrammes = programmes,
                Partners = partners,
                GalleryPreview = gallery,
                AboutSections = aboutDict,
                TotalDonationsAmount = totalDonationsAmount,
                TotalDonationsCount = totalDonationsCount,
                TotalProgrammesCount = totalProgrammesCount,
                TotalVolunteersCount = totalVolunteersCount > 0 ? totalVolunteersCount + 120 : 125, // Baseline community figure
                TotalBeneficiariesCount = 28500 + (int)(totalDonationsAmount / 10)
            };

            return View(viewModel);
        }

        // GET: /User/About or /About
        [Route("About")]
        [Route("User/About")]
        public async Task<IActionResult> About()
        {
            var sections = await _context.AboutSections
                .Where(a => a.IsActive)
                .OrderBy(a => a.DisplayOrder)
                .ToListAsync();

            var partners = await _context.Partners
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            var viewModel = new AboutViewModel
            {
                WhatWeDo = sections.FirstOrDefault(s => s.SectionKey == "WhatWeDo"),
                OurMission = sections.FirstOrDefault(s => s.SectionKey == "OurMission"),
                OurTeam = sections.FirstOrDefault(s => s.SectionKey == "OurTeam"),
                CareerWithUs = sections.FirstOrDefault(s => s.SectionKey == "CareerWithUs"),
                OurAchievements = sections.FirstOrDefault(s => s.SectionKey == "OurAchievements"),
                OurSupporters = sections.FirstOrDefault(s => s.SectionKey == "OurSupporters"),
                ReadAboutUs = sections.FirstOrDefault(s => s.SectionKey == "ReadAboutUs"),
                Partners = partners
            };

            return View(viewModel);
        }

        // GET: /User/Causes or /Causes
        [Route("Causes")]
        [Route("User/Causes")]
        public async Task<IActionResult> Causes(string? category)
        {
            var query = _context.DonationCauses.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                query = query.Where(c => c.Category == category);
            }

            var causes = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
            ViewBag.SelectedCategory = category ?? "All";
            ViewBag.Categories = await _context.DonationCauses
                .Where(c => c.IsActive)
                .Select(c => c.Category)
                .Distinct()
                .ToListAsync();

            return View(causes);
        }

        // GET: /User/CauseDetails/5
        public async Task<IActionResult> CauseDetails(int id)
        {
            var cause = await _context.DonationCauses
                .Include(c => c.Donations.Where(d => d.PaymentStatus == "Successful"))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (cause == null)
            {
                return NotFound();
            }

            var relatedCauses = await _context.DonationCauses
                .Where(c => c.Id != id && c.IsActive && c.Category == cause.Category)
                .Take(3)
                .ToListAsync();

            if (!relatedCauses.Any())
            {
                relatedCauses = await _context.DonationCauses
                    .Where(c => c.Id != id && c.IsActive)
                    .Take(3)
                    .ToListAsync();
            }

            ViewBag.RelatedCauses = relatedCauses;
            return View(cause);
        }

        // GET: /User/Donate or /Donate
        [Route("Donate")]
        [Route("User/Donate")]
        public async Task<IActionResult> Donate(int? causeId)
        {
            var availableCauses = await _context.DonationCauses
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var model = new DonateViewModel
            {
                AvailableCauses = availableCauses,
                DonationCauseId = causeId ?? (availableCauses.FirstOrDefault()?.Id ?? 0),
                Amount = 50.00m
            };

            // Pre-fill user data if logged in
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                    if (user != null)
                    {
                        model.DonorName = user.FullName;
                        model.DonorEmail = user.Email;
                        model.DonorPhone = user.Phone;
                        model.CardHolderName = user.FullName.ToUpperInvariant();
                    }
                }
            }

            if (model.DonationCauseId > 0)
            {
                model.SelectedCause = availableCauses.FirstOrDefault(c => c.Id == model.DonationCauseId);
            }

            return View(model);
        }

        // POST: /User/Donate
        [HttpPost]
        [Route("Donate")]
        [Route("User/Donate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Donate(DonateViewModel model)
        {
            var availableCauses = await _context.DonationCauses
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            model.AvailableCauses = availableCauses;
            model.SelectedCause = availableCauses.FirstOrDefault(c => c.Id == model.DonationCauseId);

            // Additional dummy card validations
            var cleanCardNumber = (model.CardNumber ?? string.Empty).Replace(" ", "").Replace("-", "");
            if (cleanCardNumber.Length < 13 || cleanCardNumber.Length > 19 || !cleanCardNumber.All(char.IsDigit))
            {
                ModelState.AddModelError("CardNumber", "Invalid card number format. Must be 13 to 19 digits.");
            }

            if (int.TryParse(model.ExpiryYear, out int expYear) && int.TryParse(model.ExpiryMonth, out int expMonth))
            {
                if (expYear < 100) expYear += 2000; // Handle 2-digit years
                var now = DateTime.UtcNow;
                if (expYear < now.Year || (expYear == now.Year && expMonth < now.Month))
                {
                    ModelState.AddModelError("ExpiryMonth", "The expiration date entered has already passed.");
                }
            }
            else
            {
                ModelState.AddModelError("ExpiryYear", "Invalid expiration year.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Mask card number: Keep last 4 digits, mask previous with '*'
            var last4 = cleanCardNumber.Length >= 4 ? cleanCardNumber.Substring(cleanCardNumber.Length - 4) : "0000";
            var masked = new string('*', 12) + last4;

            // Generate unique transaction reference
            var transRef = $"GVAID-{DateTime.UtcNow:yyyyMM}-{Random.Shared.Next(100000, 999999)}";

            // Resolve optional logged-in user
            int? userId = null;
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                    userId = user?.Id;
                }
            }

            var donation = new Donation
            {
                UserId = userId,
                DonationCauseId = model.DonationCauseId,
                Amount = model.Amount,
                DonorName = model.DonorName.Trim(),
                DonorEmail = model.DonorEmail.Trim().ToLowerInvariant(),
                DonorPhone = model.DonorPhone?.Trim(),
                CardHolderName = model.CardHolderName.Trim(),
                MaskedCardNumber = masked,
                ExpiryMonth = model.ExpiryMonth.PadLeft(2, '0'),
                ExpiryYear = model.ExpiryYear,
                TransactionReference = transRef,
                PaymentStatus = "Successful",
                DonationDate = DateTime.UtcNow,
                Notes = model.Notes?.Trim()
            };

            _context.Donations.Add(donation);

            // Increment raised amount on the associated cause
            var cause = await _context.DonationCauses.FindAsync(model.DonationCauseId);
            if (cause != null)
            {
                cause.RaisedAmount += model.Amount;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thank you! Your donation simulation was processed successfully.";
            return RedirectToAction(nameof(DonateSuccess), new { id = donation.Id });
        }

        // GET: /User/DonateSuccess/5
        public async Task<IActionResult> DonateSuccess(int id)
        {
            var donation = await _context.Donations
                .Include(d => d.DonationCause)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donation == null)
            {
                return RedirectToAction(nameof(Donate));
            }

            var receipt = new DonationReceiptViewModel
            {
                DonationId = donation.Id,
                TransactionReference = donation.TransactionReference,
                Amount = donation.Amount,
                CauseName = donation.DonationCause.Name,
                DonorName = donation.DonorName,
                DonorEmail = donation.DonorEmail,
                DonorPhone = donation.DonorPhone,
                CardHolderName = donation.CardHolderName,
                MaskedCardNumber = donation.MaskedCardNumber,
                PaymentStatus = donation.PaymentStatus,
                DonationDate = donation.DonationDate,
                Notes = donation.Notes
            };

            return View(receipt);
        }

        // GET: /User/Programmes or /Programmes
        [Route("Programmes")]
        [Route("User/Programmes")]
        public async Task<IActionResult> Programmes(string? category)
        {
            var query = _context.Programmes.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                query = query.Where(p => p.Category == category);
            }

            var programmes = await query.OrderBy(p => p.EventDate).ToListAsync();
            ViewBag.SelectedCategory = category ?? "All";
            ViewBag.Categories = await _context.Programmes
                .Where(p => p.IsActive)
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            return View(programmes);
        }

        // GET: /User/ProgrammeDetails/5
        public async Task<IActionResult> ProgrammeDetails(int id)
        {
            var programme = await _context.Programmes
                .Include(p => p.ProgrammeInterests)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (programme == null)
            {
                return NotFound();
            }

            var interestModel = new ProgrammeInterestViewModel
            {
                ProgrammeId = programme.Id,
                ProgrammeTitle = programme.Title
            };

            // Pre-fill user data if authenticated
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                    if (user != null)
                    {
                        interestModel.FullName = user.FullName;
                        interestModel.Email = user.Email;
                        interestModel.Phone = user.Phone;
                    }
                }
            }

            ViewBag.InterestModel = interestModel;
            return View(programme);
        }

        // POST: /User/ProgrammeInterest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProgrammeInterest(ProgrammeInterestViewModel model)
        {
            var programme = await _context.Programmes.FindAsync(model.ProgrammeId);
            if (programme == null || !programme.IsActive)
            {
                TempData["ErrorMessage"] = "The specified programme could not be found.";
                return RedirectToAction(nameof(Programmes));
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide all required fields to register your interest.";
                return RedirectToAction(nameof(ProgrammeDetails), new { id = model.ProgrammeId });
            }

            int? userId = null;
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                    userId = user?.Id;
                }
            }

            var interest = new ProgrammeInterest
            {
                ProgrammeId = model.ProgrammeId,
                UserId = userId,
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                Phone = model.Phone?.Trim(),
                Message = model.Message?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.ProgrammeInterests.Add(interest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your participation interest has been recorded! Our programme coordinator will reach out to you shortly.";
            return RedirectToAction(nameof(ProgrammeDetails), new { id = model.ProgrammeId });
        }

        // GET: /User/Partners or /Partners
        [Route("Partners")]
        [Route("User/Partners")]
        public async Task<IActionResult> Partners()
        {
            var partners = await _context.Partners
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            return View(partners);
        }

        // GET: /User/Gallery or /Gallery
        [Route("Gallery")]
        [Route("User/Gallery")]
        public async Task<IActionResult> Gallery(string? category)
        {
            var query = _context.GalleryItems.Where(g => g.IsActive);

            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                query = query.Where(g => g.Category == category);
            }

            var items = await query.OrderBy(g => g.DisplayOrder).ToListAsync();
            ViewBag.SelectedCategory = category ?? "All";
            ViewBag.Categories = await _context.GalleryItems
                .Where(g => g.IsActive)
                .Select(g => g.Category)
                .Distinct()
                .ToListAsync();

            return View(items);
        }

        // GET: /User/Contact or /Contact
        [Route("Contact")]
        [Route("User/Contact")]
        public async Task<IActionResult> Contact()
        {
            var settings = await _siteSettingService.GetSettingsAsync();
            var model = new ContactViewModel
            {
                SiteSetting = settings
            };

            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                    if (user != null)
                    {
                        model.Name = user.FullName;
                        model.Email = user.Email;
                        model.Phone = user.Phone;
                    }
                }
            }

            return View(model);
        }

        // POST: /User/Contact
        [HttpPost]
        [Route("Contact")]
        [Route("User/Contact")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            model.SiteSetting = await _siteSettingService.GetSettingsAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var message = new ContactMessage
            {
                Name = model.Name.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                Phone = model.Phone?.Trim(),
                Subject = model.Subject.Trim(),
                Message = model.Message.Trim(),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thank you for contacting Give-AID! Your message has been received and our team will get in touch shortly.";
            return RedirectToAction(nameof(Contact));
        }

        // GET: /User/HelpCentre or /HelpCentre
        [Route("HelpCentre")]
        [Route("User/HelpCentre")]
        public async Task<IActionResult> HelpCentre(string? search, string? category)
        {
            var query = _context.HelpItems.Where(h => h.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(h => h.Question.Contains(search) || h.Answer.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                query = query.Where(h => h.Category == category);
            }

            var faqs = await query.OrderBy(h => h.DisplayOrder).ToListAsync();
            var categories = await _context.HelpItems
                .Where(h => h.IsActive)
                .Select(h => h.Category)
                .Distinct()
                .ToListAsync();

            var settings = await _siteSettingService.GetSettingsAsync();

            var viewModel = new HelpCentreViewModel
            {
                FAQs = faqs,
                Categories = categories,
                SearchTerm = search,
                SelectedCategory = category ?? "All",
                SiteSetting = settings
            };

            return View(viewModel);
        }

        // GET: /User/InviteFriends or /InviteFriends
        [Route("InviteFriends")]
        [Route("User/InviteFriends")]
        public async Task<IActionResult> InviteFriends()
        {
            var model = new InviteViewModel();

            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                    if (user != null)
                    {
                        model.SenderName = user.FullName;
                    }
                }
            }

            return View(model);
        }

        // POST: /User/InviteFriends
        [HttpPost]
        [Route("InviteFriends")]
        [Route("User/InviteFriends")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InviteFriends(InviteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int? userId = null;
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                    userId = user?.Id;
                }
            }

            var invitation = new Invitation
            {
                SenderUserId = userId,
                SenderName = model.SenderName.Trim(),
                FriendName = model.FriendName.Trim(),
                FriendEmail = model.FriendEmail.Trim().ToLowerInvariant(),
                PersonalMessage = model.PersonalMessage?.Trim(),
                SentAt = DateTime.UtcNow
            };

            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Thank you! An invitation was logged for {model.FriendName} ({model.FriendEmail}).";
            return RedirectToAction(nameof(InviteFriends));
        }

        // GET: /User/Queries or /Queries (Authenticated users only)
        [Route("Queries")]
        [Route("User/Queries")]
        public async Task<IActionResult> Queries()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                TempData["ErrorMessage"] = "Please log in to submit and track support queries.";
                return RedirectToAction("Login", "Account", new { returnUrl = "/Queries" });
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var pastQueries = await _context.UserQueries
                .Where(q => q.UserId == user.Id || q.SenderEmail == user.Email)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            var model = new UserQueryViewModel
            {
                PastQueries = pastQueries
            };

            return View(model);
        }

        // POST: /User/Queries
        [HttpPost]
        [Route("Queries")]
        [Route("User/Queries")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Queries(UserQueryViewModel model)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account");
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                model.PastQueries = await _context.UserQueries
                    .Where(q => q.UserId == user.Id || q.SenderEmail == user.Email)
                    .OrderByDescending(q => q.CreatedAt)
                    .ToListAsync();
                return View(model);
            }

            var query = new UserQuery
            {
                UserId = user.Id,
                SenderName = user.FullName,
                SenderEmail = user.Email,
                Subject = model.Subject.Trim(),
                Message = model.Message.Trim(),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.UserQueries.Add(query);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your query has been submitted successfully. Our team will review and reply soon.";
            return RedirectToAction(nameof(Queries));
        }

        // GET: /User/Profile
        public async Task<IActionResult> Profile()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/User/Profile" });
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donations = await _context.Donations
                .Include(d => d.DonationCause)
                .Where(d => d.UserId == user.Id || d.DonorEmail == user.Email)
                .OrderByDescending(d => d.DonationDate)
                .ToListAsync();

            var interests = await _context.ProgrammeInterests
                .Include(i => i.Programme)
                .Where(i => i.UserId == user.Id || i.Email == user.Email)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var queries = await _context.UserQueries
                .Where(q => q.UserId == user.Id || q.SenderEmail == user.Email)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            var model = new ProfileViewModel
            {
                User = user,
                Donations = donations,
                ProgrammeInterests = interests,
                Queries = queries
            };

            return View(model);
        }
    }
}
