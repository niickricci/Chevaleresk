using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Chevaleresk.Models;

namespace Chevaleresk.Controllers
{
    public class AccountsController : Controller
    {
        private ChevalereskEntities db = new ChevalereskEntities();

        // GET: Joueurs
        public ActionResult Index()
        {
            return View(db.Joueurs.ToList());
        }

        // GET: Joueurs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Joueurs joueurs = db.Joueurs.Find(id);
            if (joueurs == null)
            {
                return HttpNotFound();
            }
            return View(joueurs);
        }

        // GET: Joueurs/Create
        public ActionResult Register()
        {
            return View(new Joueurs());
        }

        // POST: Joueurs/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "idJoueur,alias,nom,prenom,email,niveau, password, mdp,estAdmin,estAlchimiste,solde,Avatar, Logo")] Joueurs joueurs)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    //if(db.Joueurs.FirstOrDefault(j=>j.email == joueurs.email) != null)
                    //{

                    //}
                    //if (!string.IsNullOrEmpty(joueurs.Logo))
                    //{
                    //    string base64String = joueurs.Logo.Split(',')[1];
                    //    joueurs.Avatar = Convert.FromBase64String(base64String);
                    //}

                    //add
                    if (!string.IsNullOrEmpty(joueurs.Logo))
                    {
                        string base64String = joueurs.Logo.Split(',')[1];
                        joueurs.Avatar = Convert.FromBase64String(base64String);
                    }
                    if (string.IsNullOrEmpty(joueurs.email))
                    {
                        ModelState.AddModelError("email", "L'e-mail est requis.");
                    }
                    else if (db.Joueurs.Any(j => j.email == joueurs.email))
                    {
                        ModelState.AddModelError("email", "Cet e-mail est déjà utilisé.");
                        return View(joueurs);
                    }

                    if (string.IsNullOrEmpty(joueurs.password))
                    {
                        ModelState.AddModelError("password", "Le mot de passe est requis.");
                    }

                    if (string.IsNullOrEmpty(joueurs.alias))
                    {
                        ModelState.AddModelError("alias", "L'alias est requis.");
                    }
                    else if (db.Joueurs.Any(j => j.alias == joueurs.alias))
                    {
                        ModelState.AddModelError("alias", "Cet alias est déjà utilisé.");
                        return View(joueurs);
                    }

                    if (string.IsNullOrEmpty(joueurs.prenom))
                    {
                        ModelState.AddModelError("prenom", "Le prénom est requis.");
                    }
                    if (string.IsNullOrEmpty(joueurs.nom))
                    {
                        ModelState.AddModelError("nom", "Le nom est requis.");
                    }

                    //if (db.Joueurs.Where(j => j.email == joueurs.email).FirstOrDefault() != null)
                    //{
                    //    ModelState.AddModelError("email", "Ce email est déja utilisé.");

                    //}




                    db.ajouterJoueur(joueurs.alias, joueurs.nom, joueurs.prenom, joueurs.email, joueurs.password, joueurs.Avatar);

                    string toName = joueurs.prenom;
                    string toEmail = joueurs.email;
                    string subject = "Bienvenue chez Chevaleresk!";
                    string body = "Bienvenue sur Chevaleresk!";

                    try
                    {
                        SMTP.SendEmail(toName, toEmail, subject, body);

                    }
                    catch (Exception ex)
                    {
                    }

                    //db.Joueurs.Add(joueurs);
                    //db.SaveChanges();
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    //ModelState.AddModelError("", "Une erreur s'est produite lors de la création du joueur" + ex.Message);
                    return View(joueurs);
                }
            }

            return View(joueurs);
        }

        // GET: Joueurs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Joueurs joueurs = db.Joueurs.Find(id);
            if (joueurs == null)
            {
                return HttpNotFound();
            }
            return View(joueurs);
        }

        // POST: Joueurs/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idJoueur,alias,nom,prenom,email,niveau,mdp,estAdmin,estAlchimiste,solde,Avatar, Logo")] Joueurs joueurs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(joueurs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(joueurs);
        }

        public ActionResult Profile()
        {

            if (Session["playerID"] != null && (bool)Session["playerConnected"])
            {

                int playerID = Convert.ToInt32(Session["playerID"]);
                Joueurs joueurs = db.Joueurs.Find(playerID);
                return View(joueurs);
            }

            else
                return Redirect(@Url.Action("Login", "Accounts"));
        }
        [HttpPost]
        public ActionResult Profile([Bind(Include = "idJoueur,alias,nom,prenom,email,niveau,mdp,password,estAdmin,estAlchimiste,solde,Avatar, Logo")] Joueurs joueurs)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int playerID = Convert.ToInt32(Session["playerID"]);

                    if (string.IsNullOrEmpty(joueurs.password))
                    {
                        ModelState.AddModelError("password", "Le mot de passe est requis.");
                    }

                    if (string.IsNullOrEmpty(joueurs.alias))
                    {
                        ModelState.AddModelError("alias", "L'alias est requis.");
                    }

                    else if (db.Joueurs.Any(j => j.alias == joueurs.alias && j.idJoueur != playerID))
                    {
                        ModelState.AddModelError("alias", "Cet alias est déjà utilisé.");
                        return View(joueurs);
                    }

                    if (string.IsNullOrEmpty(joueurs.prenom))
                    {
                        ModelState.AddModelError("prenom", "Le prénom est requis.");
                    }
                    if (string.IsNullOrEmpty(joueurs.nom))
                    {
                        ModelState.AddModelError("nom", "Le nom est requis.");
                    }
                    db.modifierProfile(playerID, joueurs.alias, joueurs.nom, joueurs.prenom, joueurs.password);


                    db.SaveChanges();

                    return RedirectToAction("Logout");

                }

                catch
                {

                }

            }
            return View(joueurs);
        }
        // GET: Joueurs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Joueurs joueurs = db.Joueurs.Find(id);
            if (joueurs == null)
            {
                return HttpNotFound();
            }
            return View(joueurs);
        }

        // POST: Joueurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Joueurs joueurs = db.Joueurs.Find(id);
            db.Joueurs.Remove(joueurs);
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Joueurs joueur)
        {
            if (string.IsNullOrEmpty(joueur.email))
            {
                ModelState.AddModelError("email", "L'e-mail est requis.");
            }

            if (string.IsNullOrEmpty(joueur.password))
            {
                ModelState.AddModelError("password", "Le mot de passe est requis.");
            }

            if (ModelState.IsValid)
            {
                using (ChevalereskEntities db = new ChevalereskEntities())
                {
                    var obj = db.Joueurs.Where(j => j.email.Equals(joueur.email) && j.mdp.Equals(joueur.mdp)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["playerID"] = obj.idJoueur.ToString();
                        Session["playerAlias"] = obj.alias.ToString();
                        Session["player"] = obj;
                        Session["playerConnected"] = true;
                        if (obj.estAdmin == 1)
                        {
                            Session["isAdmin"] = true;
                        }
                        else { Session["isAdmin"] = false;}
                        if (obj.estAlchimiste == 1)
                        {
                            Session["isAlchemist"] = true;
                        }
                        else { Session["isAlchemist"] = false; }
                        Session["playerAvatar"] = "data:image/jpeg;base64," + Convert.ToBase64String(obj.Avatar);

                        TempData["NotificationType"] = "success";
                        TempData["NotificationMessage"] = $"Bienvenue {obj.alias}! Vous êtes maintenant connecté.";
                        TempData["NotificationTitle"] = "Connexion réussi";

                        return RedirectToAction("Inventory");
                    }
                    ModelState.AddModelError("", "Le courriel ou le mot de passe est invalide.");
                }
            }

            return View(joueur);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login");
        }

        public ActionResult Inventory()
        {
            if (Session["playerID"] != null && (bool)Session["playerConnected"])
            {
                int playerID = Convert.ToInt32(Session["playerID"]);

                var inventaire = db.Inventaire
                                .Include(p => p.Items)
                                .Include(p => p.Joueurs)
                                .Where(p => p.idJoueur == playerID);

                return View(inventaire.ToList());
            }
            else
            {
                return PartialView("Error");
            }
        }

    }
}
