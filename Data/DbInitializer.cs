using GiveAID.Models;
using GiveAID.Services;
using Microsoft.EntityFrameworkCore;

namespace GiveAID.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            // Seed Site Settings
            if (!await context.SiteSettings.AnyAsync())
            {
                context.SiteSettings.Add(new SiteSetting
                {
                    NgoName = "Give-AID",
                    Tagline = "Empowering Communities, Transforming Lives",
                    Email = "info@giveaid.org",
                    Phone = "+1 (555) 019-2834",
                    Address = "742 Evergreen Terrace, Suite 100",
                    CityStateZip = "Springfield, IL 62704",
                    FacebookUrl = "https://facebook.com",
                    TwitterUrl = "https://twitter.com",
                    InstagramUrl = "https://instagram.com",
                    YouTubeUrl = "https://youtube.com",
                    FooterText = "Give-AID is a certified non-profit humanitarian organization striving to alleviate poverty, empower marginalized groups, provide healthcare and education, and foster sustainable community development worldwide.",
                    LogoUrl = "/images/logo.png",
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Seed Admin User (Username: admin, Password: Admin@123)
            var adminUser = await context.Admins.FirstOrDefaultAsync(a => a.Username == "admin");
            if (adminUser == null)
            {
                context.Admins.Add(new Admin
                {
                    Username = "admin",
                    Email = "admin@giveaid.org",
                    FullName = "Chief Administrator",
                    PasswordHash = passwordHasher.HashPassword("Admin@123"),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }
            else if (adminUser.PasswordHash == "SEED_ADMIN_HASH")
            {
                adminUser.PasswordHash = passwordHasher.HashPassword("Admin@123");
            }

            // Seed Demo Community Member (Email: john.doe@example.com, Password: User@123)
            var demoMember = await context.Users.FirstOrDefaultAsync(u => u.Email == "john.doe@example.com");
            if (demoMember == null)
            {
                var demoUser = new User
                {
                    FullName = "Johnathan Doe",
                    Email = "john.doe@example.com",
                    Phone = "+1 (555) 839-1029",
                    Address = "120 Elmwood Avenue",
                    City = "Springfield",
                    Profession = "Software Engineer & Volunteer",
                    PasswordHash = passwordHasher.HashPassword("User@123"),
                    CreatedAt = DateTime.UtcNow.AddMonths(-3),
                    IsActive = true
                };

                context.Users.Add(demoUser);
            }
            else if (demoMember.PasswordHash == "SEED_USER_HASH")
            {
                demoMember.PasswordHash = passwordHasher.HashPassword("User@123");
            }

            await context.SaveChangesAsync();

            // Seed About Sections
            if (!await context.AboutSections.AnyAsync())
            {
                context.AboutSections.AddRange(
                    new AboutSection
                    {
                        SectionKey = "WhatWeDo",
                        Title = "What We Do",
                        Subtitle = "Comprehensive community development and crisis intervention",
                        Content = "At Give-AID, we design and deliver evidence-based interventions spanning child protection, accessible healthcare, vocational education, emergency disaster relief, and women empowerment. We partner directly with grass-roots community leaders to build self-sustaining ecosystems that break cyclical poverty and restore dignity.",
                        ImageUrl = "/images/about-whatwedo.jpg",
                        DisplayOrder = 1,
                        IsActive = true,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new AboutSection
                    {
                        SectionKey = "OurMission",
                        Title = "Our Mission & Vision",
                        Subtitle = "Building an equitable world with opportunities for everyone",
                        Content = "Our mission is to empower marginalized families and vulnerable communities by securing essential human rights: clean water, nutritious sustenance, high-quality education, healthcare access, and vocational self-sufficiency. We envision a society where every child dreams freely, every elder is cared for, and every family has the tools to thrive.",
                        ImageUrl = "/images/about-mission.jpg",
                        DisplayOrder = 2,
                        IsActive = true,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new AboutSection
                    {
                        SectionKey = "OurTeam",
                        Title = "Our Leadership & Volunteers",
                        Subtitle = "Passionate changemakers and dedicated humanitarian professionals",
                        Content = "Our interdisciplinary team consists of licensed social workers, pediatric healthcare practitioners, experienced disaster response coordinators, and thousands of dedicated volunteers. Together with our governing board, our staff adheres to the highest standards of financial transparency and ethical stewardship.",
                        ImageUrl = "/images/about-team.jpg",
                        DisplayOrder = 3,
                        IsActive = true,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new AboutSection
                    {
                        SectionKey = "CareerWithUs",
                        Title = "Career With Us",
                        Subtitle = "Join our mission and make a lifelong positive impact",
                        Content = "Are you passionate about social justice, international development, and humanitarian service? Give-AID offers dynamic full-time, fellowship, and volunteer opportunities across field operations, public health outreach, digital advocacy, and donor relations. We value empathy, innovative problem-solving, and collaborative spirit.",
                        ImageUrl = "/images/about-career.jpg",
                        DisplayOrder = 4,
                        IsActive = true,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new AboutSection
                    {
                        SectionKey = "OurAchievements",
                        Title = "Our Key Milestones & Impact",
                        Subtitle = "Measurable outcomes achieved through donor generosity",
                        Content = "Over the past decade, Give-AID has provided over 250,000 hot nutritious meals, built 38 solar-powered clean water filtration units in underserved villages, sponsored continuous schooling for 4,200 children, conducted 110 free mobile health clinics, and provided seed-funding to 1,500 women-led cottage businesses.",
                        ImageUrl = "/images/about-achievements.jpg",
                        DisplayOrder = 5,
                        IsActive = true,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new AboutSection
                    {
                        SectionKey = "OurSupporters",
                        Title = "Our Supporters & Philanthropists",
                        Subtitle = "The generous hearts that fuel every single campaign",
                        Content = "We are profoundly grateful to our individual donors, family trusts, international NGOs, and socially responsible corporations. Your generous contributions provide the lifeblood of our initiatives. Every dollar received is diligently audited and deployed directly to programmatic welfare.",
                        ImageUrl = "/images/about-supporters.jpg",
                        DisplayOrder = 6,
                        IsActive = true,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new AboutSection
                    {
                        SectionKey = "ReadAboutUs",
                        Title = "Read About Us & Our Governance",
                        Subtitle = "Operating with uncompromising transparency and accountability",
                        Content = "Give-AID is an independently audited 501(c)(3) equivalent NGO. Over 88% of all programmatic funds directly reach our beneficiaries in the field, with minimal overhead. We publish comprehensive annual impact reports, quarterly financial filings, and independent monitoring assessments for public scrutiny.",
                        ImageUrl = "/images/about-read.jpg",
                        DisplayOrder = 7,
                        IsActive = true,
                        UpdatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Donation Causes
            if (!await context.DonationCauses.AnyAsync())
            {
                context.DonationCauses.AddRange(
                    new DonationCause
                    {
                        Name = "Children Welfare & Nutrition",
                        Category = "Children",
                        ShortDescription = "Providing essential daily nutrition, protective clothing, and nurturing shelter for vulnerable and orphaned children.",
                        FullDescription = "Malnutrition impairs cognitive growth and physical vitality. Through this campaign, Give-AID operates daily nutrition centres, emergency infant milk distribution, and seasonal clothing packages for children living in high-risk environments.",
                        TargetAmount = 50000.00m,
                        RaisedAmount = 32850.00m,
                        ImageUrl = "/images/cause-children.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-6)
                    },
                    new DonationCause
                    {
                        Name = "Education for Every Child",
                        Category = "Education",
                        ShortDescription = "Supplying school tuition fees, textbooks, digital tablets, and school uniforms to low-income students.",
                        FullDescription = "Education is the single most effective ladder out of intergenerational poverty. Your donations fund complete school sponsorship packages, qualified remedial teachers, classroom supplies, and computer labs for rural schools.",
                        TargetAmount = 40000.00m,
                        RaisedAmount = 26400.00m,
                        ImageUrl = "/images/cause-education.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-5)
                    },
                    new DonationCause
                    {
                        Name = "Disabled Persons Accessibility & Care",
                        Category = "Disability",
                        ShortDescription = "Distributing wheelchairs, prosthetic aids, hearing devices, and accessible skill training for disabled individuals.",
                        FullDescription = "People with physical and sensory disabilities face systemic barriers to employment and mobility. We provide custom assistive mobility devices, physical rehabilitation therapy, and specialized digital vocational skills.",
                        TargetAmount = 35000.00m,
                        RaisedAmount = 19200.00m,
                        ImageUrl = "/images/cause-disabled.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-4)
                    },
                    new DonationCause
                    {
                        Name = "Women Welfare & Artisan Micro-Grants",
                        Category = "Women",
                        ShortDescription = "Empowering single mothers and female artisans through vocational training, micro-grants, and legal support.",
                        FullDescription = "When women thrive, entire communities prosper. Give-AID provides vocational tailoring, handicraft training, business literacy classes, and zero-interest micro-grants to empower women into self-sufficient entrepreneurs.",
                        TargetAmount = 45000.00m,
                        RaisedAmount = 37100.00m,
                        ImageUrl = "/images/cause-women.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-4)
                    },
                    new DonationCause
                    {
                        Name = "Youth Development & Vocational Trades",
                        Category = "Youth",
                        ShortDescription = "Offering coding bootcamps, electrical/plumbing trade apprenticeships, and youth sports mentorship.",
                        FullDescription = "Unemployment among youths breeds despair. Our youth innovation hubs teach high-demand digital skills, certified trade mechanics, leadership workshops, and organized community sports leagues.",
                        TargetAmount = 30000.00m,
                        RaisedAmount = 14800.00m,
                        ImageUrl = "/images/cause-youth.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-3)
                    },
                    new DonationCause
                    {
                        Name = "Elderly Healthcare & Social Protection",
                        Category = "Elderly",
                        ShortDescription = "Ensuring routine chronic prescription medicines, nutritional support, and companionship visits for seniors.",
                        FullDescription = "Many elderly citizens lack family support or pension security. We deliver weekly door-to-door medical checkups, essential chronic medications (hypertension, diabetes), warm blankets, and community dining clubs.",
                        TargetAmount = 25000.00m,
                        RaisedAmount = 18950.00m,
                        ImageUrl = "/images/cause-elderly.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-3)
                    },
                    new DonationCause
                    {
                        Name = "Emergency Medical Relief & Clean Water",
                        Category = "Healthcare",
                        ShortDescription = "Installing deep community water boreholes and dispatching mobile medical clinics to crisis zones.",
                        FullDescription = "Waterborne diseases remain a leading cause of preventable illness. This fund installs deep-well solar boreholes providing safe drinking water to thousands, alongside mobile doctor clinics dispensing life-saving medicine.",
                        TargetAmount = 60000.00m,
                        RaisedAmount = 48500.00m,
                        ImageUrl = "/images/cause-water.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-2)
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Programmes
            if (!await context.Programmes.AnyAsync())
            {
                context.Programmes.AddRange(
                    new Programme
                    {
                        Title = "Annual Winter Warmth Drive 2026",
                        Category = "Relief",
                        ShortDescription = "Community distribution of 5,000 thermal blankets, winter jackets, and heaters to unhoused families.",
                        FullDescription = "As temperatures plummet, vulnerable homeless citizens and under-insulated households face severe hypothermia risks. Give-AID volunteers distribute heavy-duty thermal fleece blankets, thermal socks, winter coats, and hot soup across key urban and peri-urban hubs.",
                        EventDate = DateTime.UtcNow.AddDays(25),
                        Location = "Metro Community Center, North Wing, Springfield",
                        Status = "Upcoming",
                        ImageUrl = "/images/prog-winter.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-1)
                    },
                    new Programme
                    {
                        Title = "Mobile Health & Free Eye Care Clinic",
                        Category = "Healthcare",
                        ShortDescription = "Free medical consultations, prescription optical glasses, and diabetes screenings for 1,200 residents.",
                        FullDescription = "A team of 15 volunteer physicians, optometrists, and registered nurses will conduct comprehensive triage, pediatric examinations, cataract evaluations, and distribute free corrective eyeglasses and antibiotics.",
                        EventDate = DateTime.UtcNow.AddDays(40),
                        Location = "Riverside High School Gymnasium, Springfield",
                        Status = "Upcoming",
                        ImageUrl = "/images/prog-health.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    },
                    new Programme
                    {
                        Title = "Clean Water for Rural Schools Initiative",
                        Category = "Environment",
                        ShortDescription = "Commissioning of 8 new solar-powered ultrafiltration clean drinking water stations in public schools.",
                        FullDescription = "Access to clean water drastically cuts student absenteeism. We are hosting the ribbon-cutting and hygiene training workshops for faculty, parents, and over 3,000 schoolchildren.",
                        EventDate = DateTime.UtcNow.AddDays(55),
                        Location = "Green Valley Educational District, Sector 4",
                        Status = "Upcoming",
                        ImageUrl = "/images/prog-water.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-15)
                    },
                    new Programme
                    {
                        Title = "Youth Digital Literacy & Coding Bootcamp",
                        Category = "Education",
                        ShortDescription = "A free 6-week weekend training in web programming and computer basics for disadvantaged youth.",
                        FullDescription = "Equipping young adults aged 16-24 with practical skills in HTML, CSS, JavaScript, and computer diagnostics to jumpstart their career prospects in the modern economy.",
                        EventDate = DateTime.UtcNow.AddDays(70),
                        Location = "Give-AID Tech Hub, 3rd Floor Auditorium",
                        Status = "Upcoming",
                        ImageUrl = "/images/prog-coding.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Programme
                    {
                        Title = "Women Artisan Skill & Micro-Business Fair",
                        Category = "Women Welfare",
                        ShortDescription = "Exhibition showcasing handmade crafts, textiles, and organic foods produced by our women trainees.",
                        FullDescription = "Celebrate and purchase exquisite hand-loomed goods, embroidery, pottery, and culinary treats directly from women entrepreneurs enrolled in our livelihood empowerment programs.",
                        EventDate = DateTime.UtcNow.AddDays(85),
                        Location = "Centennial City Plaza, Pavilion B",
                        Status = "Upcoming",
                        ImageUrl = "/images/prog-women.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new Programme
                    {
                        Title = "Weekend Food Relief & Community Kitchen",
                        Category = "Relief",
                        ShortDescription = "Serving 800 freshly cooked warm meals and distributing grocery survival packs to elderly pensioners.",
                        FullDescription = "Join our chef teams and distribution volunteers as we prepare, package, and deliver nutrient-dense hot meals and monthly pantry dry goods to senior housing complexes.",
                        EventDate = DateTime.UtcNow.AddDays(12),
                        Location = "Hope Baptist Hall, East Springfield",
                        Status = "Upcoming",
                        ImageUrl = "/images/prog-food.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Partners
            if (!await context.Partners.AnyAsync())
            {
                context.Partners.AddRange(
                    new Partner
                    {
                        Name = "Global Health Outreach Alliance",
                        Category = "NGO / Healthcare",
                        Description = "International medical consortium collaborating on pharmaceutical supplies, triage equipment, and volunteer doctor delegations.",
                        WebsiteUrl = "https://example.org/global-health",
                        LogoUrl = "/images/partner-gho.png",
                        DisplayOrder = 1,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-6)
                    },
                    new Partner
                    {
                        Name = "Beacon Education Foundation",
                        Category = "Academic Foundation",
                        Description = "Philanthropic endowment supplying curriculum licenses, classroom books, and digital educational hardware.",
                        WebsiteUrl = "https://example.org/beacon-education",
                        LogoUrl = "/images/partner-beacon.png",
                        DisplayOrder = 2,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-5)
                    },
                    new Partner
                    {
                        Name = "Apex Community Bank CSR",
                        Category = "Corporate Partner",
                        Description = "Providing dollar-for-dollar employee matching grants, financial literacy workshops, and micro-business credit lines.",
                        WebsiteUrl = "https://example.org/apex-bank",
                        LogoUrl = "/images/partner-apex.png",
                        DisplayOrder = 3,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-5)
                    },
                    new Partner
                    {
                        Name = "CleanWorld Water Initiative",
                        Category = "Environmental NGO",
                        Description = "Specializing in geological groundwater survey, solar pump installations, and village sanitation training.",
                        WebsiteUrl = "https://example.org/clean-world",
                        LogoUrl = "/images/partner-cleanworld.png",
                        DisplayOrder = 4,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-4)
                    },
                    new Partner
                    {
                        Name = "Horizon Tech Innovations",
                        Category = "Corporate Tech Sponsor",
                        Description = "Donating refurbished laptops, cloud services, and software mentorship for our youth coding bootcamps.",
                        WebsiteUrl = "https://example.org/horizon-tech",
                        LogoUrl = "/images/partner-horizon.png",
                        DisplayOrder = 5,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-3)
                    },
                    new Partner
                    {
                        Name = "United Humanity Network",
                        Category = "International Aid Federation",
                        Description = "Rapid disaster response partner facilitating logistics and international air freight during regional crises.",
                        WebsiteUrl = "https://example.org/united-humanity",
                        LogoUrl = "/images/partner-humanity.png",
                        DisplayOrder = 6,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-2)
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Help FAQs
            if (!await context.HelpItems.AnyAsync())
            {
                context.HelpItems.AddRange(
                    new HelpItem
                    {
                        Question = "How does Give-AID ensure my donation reaches those in need?",
                        Answer = "Give-AID practices strict fiduciary transparency. Over 88% of all funds are directed straight into active field programmes. Our financial statements are audited by independent certified public accountants and filed publicly annually.",
                        Category = "Donations",
                        DisplayOrder = 1,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new HelpItem
                    {
                        Question = "Is my online donation secure?",
                        Answer = "Yes! We utilize 256-bit SSL encryption. In this demonstration environment, card processing is fully simulated—no actual card numbers or CVVs are permanently stored on our servers, ensuring absolute privacy.",
                        Category = "Donations",
                        DisplayOrder = 2,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new HelpItem
                    {
                        Question = "How do I volunteer or participate in upcoming programmes?",
                        Answer = "Navigate to our 'Programmes' page, select any upcoming initiative that resonates with you, and click 'Participate / I am Interested'. Fill in your details and our volunteer coordinator will contact you with orientation schedules.",
                        Category = "Volunteering",
                        DisplayOrder = 3,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new HelpItem
                    {
                        Question = "Can I receive a tax deduction certificate for my contributions?",
                        Answer = "Yes! All registered donors receive an automated digital transaction receipt immediately upon successful payment. At year-end, registered members can view and download their consolidated annual giving statement in their profile.",
                        Category = "Donations",
                        DisplayOrder = 4,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new HelpItem
                    {
                        Question = "How do I register an account as a community member?",
                        Answer = "Click 'Register' in the top navigation bar, complete the brief form with your contact details, and submit. You will immediately be able to log in, track your donations, raise support queries, and RSVP to events.",
                        Category = "Account",
                        DisplayOrder = 5,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new HelpItem
                    {
                        Question = "How can our company become an official Give-AID Partner?",
                        Answer = "We welcome CSR collaborations, grant partnerships, and employee giving drives! Please contact our partnerships director via the Contact Us form or email us directly at partners@giveaid.org.",
                        Category = "Partnership",
                        DisplayOrder = 6,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new HelpItem
                    {
                        Question = "How can I raise a query or request assistance?",
                        Answer = "Registered members can log into their account and click 'Queries' in the navbar to open a formal inquiry. You can track the status in real time as our administration reviews and responds to your query.",
                        Category = "Support",
                        DisplayOrder = 7,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new HelpItem
                    {
                        Question = "Can I invite my friends and colleagues to join Give-AID?",
                        Answer = "Yes! Use our 'Invite Friends' tool accessible from the main navigation. Simply provide your friend's name and email along with a personalized note to introduce them to our humanitarian community.",
                        Category = "Community",
                        DisplayOrder = 8,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Gallery
            if (!await context.GalleryItems.AnyAsync())
            {
                context.GalleryItems.AddRange(
                    new GalleryItem
                    {
                        Title = "Child Nutrition Center Daily Meal Distribution",
                        Category = "Children",
                        Description = "Volunteers serving hot fortified meals to 300+ primary school students.",
                        ImageUrl = "/images/gallery-1.jpg",
                        DisplayOrder = 1,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-2)
                    },
                    new GalleryItem
                    {
                        Title = "Solar Clean Water Filtration Commissioning",
                        Category = "Water",
                        Description = "Villagers celebrating the turning on of their first clean tap water borehole.",
                        ImageUrl = "/images/gallery-2.jpg",
                        DisplayOrder = 2,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-2)
                    },
                    new GalleryItem
                    {
                        Title = "Youth Robotics & Computer Skills Workshop",
                        Category = "Education",
                        Description = "High school students collaborating on beginner robotics kits.",
                        ImageUrl = "/images/gallery-3.jpg",
                        DisplayOrder = 3,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-1)
                    },
                    new GalleryItem
                    {
                        Title = "Mobile Eyecare & Reading Glasses Camp",
                        Category = "Healthcare",
                        Description = "Optometrist fitting customized reading glasses for an elderly grandmother.",
                        ImageUrl = "/images/gallery-4.jpg",
                        DisplayOrder = 4,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddMonths(-1)
                    },
                    new GalleryItem
                    {
                        Title = "Women Weaving & Textile Cooperative",
                        Category = "Women",
                        Description = "Artisans creating traditional fabrics funded through our micro-grant program.",
                        ImageUrl = "/images/gallery-5.jpg",
                        DisplayOrder = 5,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    },
                    new GalleryItem
                    {
                        Title = "Winter Blanket & Thermal Kit Packaging",
                        Category = "Relief",
                        Description = "Over 1,000 thermal supply bundles packaged by enthusiastic volunteers.",
                        ImageUrl = "/images/gallery-6.jpg",
                        DisplayOrder = 6,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Sample Donations and Queries for testing/dashboard demo
            if (!await context.Donations.AnyAsync())
            {
                var cause1 = await context.DonationCauses.FirstOrDefaultAsync(c => c.Name.Contains("Children"));
                var cause2 = await context.DonationCauses.FirstOrDefaultAsync(c => c.Name.Contains("Education"));
                var cause3 = await context.DonationCauses.FirstOrDefaultAsync(c => c.Name.Contains("Water"));
                var demoUser = await context.Users.FirstOrDefaultAsync();

                if (cause1 != null && cause2 != null)
                {
                    context.Donations.AddRange(
                        new Donation
                        {
                            UserId = demoUser?.Id,
                            DonationCauseId = cause1.Id,
                            Amount = 250.00m,
                            DonorName = "Johnathan Doe",
                            DonorEmail = "john.doe@example.com",
                            DonorPhone = "+1 (555) 839-1029",
                            CardHolderName = "JOHNATHAN DOE",
                            MaskedCardNumber = "************4242",
                            ExpiryMonth = "12",
                            ExpiryYear = "2027",
                            TransactionReference = "GVAID-2026-100234",
                            PaymentStatus = "Successful",
                            DonationDate = DateTime.UtcNow.AddDays(-5),
                            Notes = "In honor of all teachers and children."
                        },
                        new Donation
                        {
                            UserId = null,
                            DonationCauseId = cause2.Id,
                            Amount = 100.00m,
                            DonorName = "Sarah Jenkins",
                            DonorEmail = "sarah.j@example.org",
                            DonorPhone = "+1 (555) 321-9988",
                            CardHolderName = "SARAH JENKINS",
                            MaskedCardNumber = "************8812",
                            ExpiryMonth = "08",
                            ExpiryYear = "2028",
                            TransactionReference = "GVAID-2026-100235",
                            PaymentStatus = "Successful",
                            DonationDate = DateTime.UtcNow.AddDays(-3),
                            Notes = "Keep up the wonderful educational work!"
                        },
                        new Donation
                        {
                            UserId = null,
                            DonationCauseId = cause3 != null ? cause3.Id : cause1.Id,
                            Amount = 500.00m,
                            DonorName = "Robert Sterling",
                            DonorEmail = "r.sterling@corporate-giving.com",
                            DonorPhone = "+1 (555) 902-1144",
                            CardHolderName = "ROBERT STERLING",
                            MaskedCardNumber = "************5531",
                            ExpiryMonth = "04",
                            ExpiryYear = "2029",
                            TransactionReference = "GVAID-2026-100236",
                            PaymentStatus = "Successful",
                            DonationDate = DateTime.UtcNow.AddDays(-1),
                            Notes = "Corporate matching grant donation."
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }

            // Seed Sample User Query
            if (!await context.UserQueries.AnyAsync())
            {
                var demoUser = await context.Users.FirstOrDefaultAsync();
                context.UserQueries.Add(new UserQuery
                {
                    UserId = demoUser?.Id,
                    SenderName = demoUser?.FullName ?? "Johnathan Doe",
                    SenderEmail = demoUser?.Email ?? "john.doe@example.com",
                    Subject = "Corporate Matching for Donations",
                    Message = "Hello Give-AID team, my employer matches charitable donations 1:1. Where can I email our corporate match form so the donation to the Children Welfare campaign is doubled?",
                    AdminResponse = "Hello Johnathan! Thank you so much for your generosity. You can email the matching form directly to finance@giveaid.org with your transaction reference GVAID-2026-100234, and our treasury team will sign and return it within 24 hours.",
                    Status = "Resolved",
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    RespondedAt = DateTime.UtcNow.AddDays(-3)
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
