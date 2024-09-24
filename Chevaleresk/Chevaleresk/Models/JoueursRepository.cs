using Chevaleresk.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;

namespace Chevaleresk.Models
{
    public class JoueursRepository
    {
        private ChevalereskEntities DB = new ChevalereskEntities();


        //public Joueurs Findjoueur(int id)
        //{
        //    try
        //    {
        //        Joueurs joueur = DB.Joueurs.FirstOrDefault(j =>j.idJoueur == id);

        //        if (joueur != null)
        //        {
        //            //joueur.ConfirmEmail = joueur.Email;
        //            //joueur.ConfirmPassword = joueur.Password;
        //        }
        //        return joueur;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Find joueur failed : Message - {ex.Message}");
        //        return null;
        //    }
        //}
        public IEnumerable<Joueurs> SortedJoueurs()
        {
            return DB.Joueurs.ToList().OrderBy(u => u.prenom).ThenBy(u => u.nom);
        }

        //public bool ChangeEmail(string code)
        //{
        //    UnverifiedEmail unverifiedEmail = FindUnverifiedEmail(code);

        //    if (unverifiedEmail != null)
        //    {
        //        joueur joueur = Get(unverifiedEmail.joueurId);

        //        if (joueur != null)
        //        {
        //            try
        //            {
        //                BeginTransaction();
        //                joueur.Email = joueur.ConfirmEmail = unverifiedEmail.Email;
        //                joueur.Verified = true;
        //                base.Update(joueur);
        //                DB.UnverifiedEmails.Delete(unverifiedEmail.Id);
        //                EndTransaction();
        //                return true;
        //            }
        //            catch (Exception ex)
        //            {
        //                System.Diagnostics.Debug.WriteLine($"Verify_joueur failed : Message - {ex.Message}");
        //                EndTransaction();
        //            }
        //        }
        //    }
        //    return false;
        //}
        public bool EmailAvailable(string email, int excludedId = 0)
        {
            Joueurs joueur = DB.Joueurs.ToList().Where(u => u.email.ToLower() == email.ToLower()).FirstOrDefault();
            if (joueur == null)
                return true;
            else
                if (joueur.idJoueur != excludedId)
                return joueur.email.ToLower() != email.ToLower();
            return true;
        }

        public bool EmailExist(string email)
        {
            return DB.Joueurs.ToList().Where(u => u.email.ToLower() == email.ToLower()).FirstOrDefault() != null;
        }

        public Joueurs Getjoueur(string email, string mdp)
        {
            Byte[] pwd = null;
            using (SHA512 sha512 = SHA512.Create())
            {
                pwd = sha512.ComputeHash(Encoding.UTF8.GetBytes(mdp));
            }
            Joueurs joueur = DB.Joueurs.ToList().Where(u => (u.email.ToLower() == email.ToLower()) && (u.mdp == pwd)).FirstOrDefault();

            if (joueur != null)
            {
                return joueur;
            }
            else
            {
                return null;
            }
        }

        public int GetSolde(int id)
        {
            Joueurs joueur = DB.Joueurs.Find(id);
            var solde = joueur.solde;

            return solde;
        }
         public bool IsAlchemist(int id)
        {
         
            Joueurs joueur = DB.Joueurs.Find(id);
            if(joueur == null)
            {
                return false;
            }
            bool isAlchemist = joueur.estAlchimiste == 1 ? true : false;

            return isAlchemist;
        }
        public bool IsAdmin(int id)
        {

            Joueurs joueur = DB.Joueurs.Find(id);
            if (joueur == null)
            {
                return false;
            }
            bool isAlchemist = joueur.estAdmin == 1 ? true : false;

            return isAlchemist;
        }
        public string GetNiveau(int id)
        {
            Joueurs joueur = DB.Joueurs.Find(id);
            var niveau =  joueur.niveau;
            return niveau;  
        }

        //Retourne la qt d'un item de l'inventaire du joueur
        public int GetQteItemInventaire(int idJoueur,  int idItem)
        {
            Joueurs joueurs = DB.Joueurs.Find(idJoueur);

            var item = DB.Inventaire.Where(i => i.idItem == idItem && i.idJoueur == idJoueur).FirstOrDefault();
            int qteItem; 

            if (item == null)
            {
                qteItem = 0;
            }
            else
            {
                qteItem = item.qtInventaire;
            }

            return qteItem;
        }

        public bool IsUsernameUnique(string username)
        {
            var joueur = DB.Joueurs.Where(j => j.alias == username).FirstOrDefault();
            if (joueur == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasItem(int idJoueur, int idItem    )
        {
            var item = DB.Inventaire.Where(i => i.idItem == idItem && i.idJoueur == idJoueur).FirstOrDefault();
            if (item == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool HasNotReviewedItem(int idJoueur, int idItem)
        {
            var item = DB.Critique.Where(i => i.idItem == idItem && i.idJoueur == idJoueur).FirstOrDefault();
            if (item == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetAlias(int id)
        {
            Joueurs joueur = DB.Joueurs.Find(id);
            if (joueur == null)
            {
                return null;
            }

            var alias = joueur.alias;

            return alias;
        }

        //public checkAlias(int idJoueur, string alias)
        //{
        //    //DB.Joueurs.Any(j=> j.alias == alias && j.idJoueur != idJoueur)
        //}
    }
}