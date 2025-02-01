using Microsoft.AspNetCore.Mvc;
using Forms.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Forms.Controllers;

public class HomeController : Controller
{


    public HomeController()
    {

    }

    public IActionResult Index(string searchString, string category)
    {
        //ViewBag.Categories=new SelectList(Repository.Categories,"CategoryId","Name",category);      
        var products = Repository.Products;
        if (!String.IsNullOrEmpty(searchString))
        {
            ViewBag.SearchString = searchString;
            products = products.Where(p => p.Name.ToLower().Contains(searchString.ToLower())).ToList();
        }
        if (!String.IsNullOrEmpty(category) && category != "0")
        {
            products = products.Where(p => p.CategoryId == int.Parse(category)).ToList();
        }

        var model = new ProductViewModel
        {
            Products = products,
            Categories = Repository.Categories,
            SelectedCategory = category
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Product model, IFormFile? imageFile)
    {
        if (imageFile == null)
        {
            ModelState.AddModelError("", "Gecerli bir resim secin");
            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
            return View(model);
        }
        var allowedExtentions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(imageFile.FileName);
        var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
        if (imageFile != null)
        {
            if (!allowedExtentions.Contains(extension))
            {
                ModelState.AddModelError("", "Gecerli bir resim secin");
            }
        }

        if (ModelState.IsValid)
        {
            if (imageFile != null)
            {

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                model.Image = randomFileName;
                model.ProductId = Repository.Products.Count + 1;
                Repository.CreateProduct(model);
                return RedirectToAction("Index");
            }

        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);

    }

    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
        if (entity == null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product model, IFormFile? imageFile)
    {
        if (id != model.ProductId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName);
                var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                model.Image = randomFileName;
            }
            Repository.EditProduct(model);
            return RedirectToAction("Index");
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);
    }
    public IActionResult Delete(int? id)
    {
        if(id==null)
        {
            return NotFound();
        }

        var entity = Repository.Products.FirstOrDefault(p =>p.ProductId==id);
        if(entity==null)
        {
                 return NotFound();
        }
        return View("DeleteConfirm", entity);         
    }
    [HttpPost]
    public IActionResult Delete(int id, int productId)
    {
        if(id!=productId)
        {
            return NotFound();
        }
      var entity = Repository.Products.FirstOrDefault(p =>p.ProductId==productId);
        if(entity==null)
        {
                 return NotFound();
        }
        Repository.DeleteProduct(entity);
        return RedirectToAction("Index");
    }
 [HttpPost]
    public IActionResult EditProducts(List<Product> products)
    {
        foreach(var p in products)
        {
            Repository.EditIsActive(p);
        }
        return RedirectToAction("Index");
    }
}
