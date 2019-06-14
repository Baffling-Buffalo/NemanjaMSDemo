using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCClient.Attributes;
using MVCClient.Services;
using MVCClient.ViewModels;

namespace MVCClient.Controllers
{
    public class Api1Controller : Controller
    {
        private IApi1Service api1Service;

        public Api1Controller(IApi1Service api1Service)
        {
            this.api1Service = api1Service;
        }

        // GET: Api1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Api1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ModelValidationFilter]
        public async Task<ActionResult> Create(Api1Object model)
        {
            try
            {
                await api1Service.CreateData(model);
                return RedirectToAction("Index","Home");
            }
            catch
            {
                return View();
            }
        }

        // GET: Api1/Edit/5
        public ActionResult Edit(int? id)
        {
            var data = api1Service.GetData(id);
            return View();
        }

        // POST: Api1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index", nameof(HomeController));
            }
            catch
            {
                return View();
            }
        }

        // GET: Api1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Api1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index", nameof(HomeController));
            }
            catch
            {
                return View();
            }
        }
    }
}