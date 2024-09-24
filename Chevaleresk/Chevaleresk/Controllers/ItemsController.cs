using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Chevaleresk.Models;

namespace Chevaleresk.Views
{
    public class ItemsController : Controller
    {
        private ChevalereskEntities db = new ChevalereskEntities();

        // GET: Items
        public ActionResult Index()
        {
            var items = db.Items.Include(i => i.elements).Include(i => i.Magasin).Include(i => i.Potion).Include(i => i.Armure).Include(i => i.Arme);
            return View(items.ToList());
        }

        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Items items = db.Items.Find(id);
            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            //ViewBag.idItem = new SelectList(db.elements, "idItem", "rarete");
            //ViewBag.idItem = new SelectList(db.Magasin, "idItem", "idItem");
            //ViewBag.idItem = new SelectList(db.Potion, "idItem", "effet");

                // Initialiser une nouvelle instance de Items
            var newItem = new Items();

            newItem.Arme = new Arme();
            newItem.Armure = new Armure();
            newItem.Potion = new Potion();
            newItem.elements = new elements();

            return View(newItem);
        }

        // POST: Items/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idItem,nom,qtStock,prix,photo,itemLogo,typeItem,flagDispo,Arme,Armure,Potion,elements")] Items items)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(items.ItemLogo))
                {
                    string base64String = items.ItemLogo.Split(',')[1];
                    items.photo = Convert.FromBase64String(base64String);
                }
                if (items.typeItem == "C")
                {
                    db.ajouterArmure(items.nom, Convert.ToInt32(items.qtStock), Convert.ToInt32(items.prix), items.photo, items.Armure.matiere, items.Armure.taille);
                }
                if (items.typeItem == "A")
                {
                    db.ajouterArme(items.nom, Convert.ToInt32(items.qtStock), Convert.ToInt32(items.prix), items.photo, items.Arme.efficacite, items.Arme.genre, items.Arme.description);
                    db.SaveChanges();
                }
                if (items.typeItem == "P")
                {
                    db.ajouterPotion(items.nom, Convert.ToInt32(items.qtStock), Convert.ToInt32(items.prix), items.photo, items.Potion.effet, Convert.ToInt32(items.Potion.duree));
                }
                if (items.typeItem == "E")
                {
                    db.ajouterElement(items.nom, Convert.ToInt32(items.qtStock), Convert.ToInt32(items.prix), items.photo, items.elements.rarete, items.elements.dangerosite, items.elements.typeElement);
                }
                return RedirectToAction("Index");
            }

            //ViewBag.idItem = new SelectList(db.elements, "idItem", "rarete", items.idItem);
            //ViewBag.idItem = new SelectList(db.Magasin, "idItem", "idItem", items.idItem);
            //ViewBag.idItem = new SelectList(db.Potion, "idItem", "effet", items.idItem);
            //ViewBag.idItem = new SelectList(db.Armure, "idItem", "taille", items.idItem);
            //ViewBag.idItem = new SelectList(db.Arme, "idItem", "genre", items.idItem);
            return View(items);
        }

        // GET: Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Items items = db.Items.Find(id);
            if (items == null)
            {
                return HttpNotFound();
            }
            ViewBag.idItem = new SelectList(db.elements, "idItem", "rarete", items.idItem);
            ViewBag.idItem = new SelectList(db.Magasin, "idItem", "idItem", items.idItem);
            ViewBag.idItem = new SelectList(db.Potion, "idItem", "effet", items.idItem);
            return View(items);
        }

        // POST: Items/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idItem,nom,qtStock,prix,photo,typeItem,flagDispo")] Items items)
        {
            if (ModelState.IsValid)
            {
                db.Entry(items).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idItem = new SelectList(db.elements, "idItem", "rarete", items.idItem);
            ViewBag.idItem = new SelectList(db.Magasin, "idItem", "idItem", items.idItem);
            ViewBag.idItem = new SelectList(db.Potion, "idItem", "effet", items.idItem);
            return View(items);
        }

        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Items items = db.Items.Find(id);
            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Items items = db.Items.Find(id);
            db.Items.Remove(items);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
