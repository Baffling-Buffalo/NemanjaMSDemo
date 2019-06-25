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

        public async Task<IActionResult> Index(int? id = null)
        {
            List<Api1Object> api1Objects = await api1Service.GetData(id);
            return View(api1Objects);
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
        public async Task<ActionResult> Edit(int? id)
        {
            var data = await api1Service.GetData(id);
            return View(data.FirstOrDefault());
        }

        // POST: Api1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Api1Object model)
        {
            try
            {
                var data = await api1Service.GetData(model.Id);
                if (data?.FirstOrDefault() != null)
                {
                    await api1Service.UpdateData(model);
                }
                return RedirectToAction("Index", "api1");
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

                return RedirectToAction("Index", "api1");
            }
            catch
            {
                return View();
            }
        }
    }
}