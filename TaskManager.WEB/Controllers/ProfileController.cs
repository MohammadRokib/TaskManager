﻿using Microsoft.AspNetCore.Mvc;

namespace TaskManager.WEB.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
