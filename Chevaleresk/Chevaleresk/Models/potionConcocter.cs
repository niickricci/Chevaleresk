//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chevaleresk.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class potionConcocter
    {
        public int idJoueur { get; set; }
        public int qtPotionConcocter { get; set; }
    
        public virtual Joueurs Joueurs { get; set; }
    }
}
