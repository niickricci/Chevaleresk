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
    public class ItemssRepository
    {
        private ChevalereskEntities DB = new ChevalereskEntities();

        public double AvgItemReviews(int idItem)
        {
            if (DB.Items.Find(idItem) == null)
            {
                return 0;
            }
            var reviews = DB.Critique.Where(r => r.idItem == idItem);
            if (reviews.Count() == 0)
            {
                return 0;
            }
            return reviews.Average(r => r.nbEtoile);
        }

        public int TotalReviews(int idItem)
        {
            if (DB.Items.Find(idItem) == null)
            {
                return 0;
            }
            var reviews = DB.Critique.Where(r => r.idItem == idItem);
            if (reviews.Count() == 0)
            {
                return 0;
            }
            return reviews.Count();
        }

        public int TotalReviewsByStars(int idItem, int nbEtoile)
        {
            if (DB.Items.Find(idItem) == null)
            {
                return 0;
            }
            var reviews = DB.Critique.Where(r => r.idItem == idItem && r.nbEtoile == nbEtoile);
            if (reviews.Count() == 0)
            {
                return 0;
            }
            return reviews.Count();
        }
    }
}