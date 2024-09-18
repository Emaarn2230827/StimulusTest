﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STIMULUS_V2.Shared.Models.Entities
{
    public class Noeud
    {
        public int NoeudId { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string Nom { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string Description { get; set; }

        public DateTime Disponibilite { get; set; } = DateTime.Now;

        public bool Obligatoire { get; set; } = false;

        public int Status { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public double PosX { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public double PosY { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public double Rayon { get; set; } = 40;

        [Column(TypeName = "nvarchar(50)")]
        public NoeudType Type { get; set; }

        [ForeignKey("Graphe")]
        public int? GrapheId { get; set; }

        public Graphe? Graphe { get; set; }

        [ForeignKey("NoeudParent")]
        public int? NoeudParentId { get; set; }

        public Noeud? NoeudParent { get; set; }
    }

    public enum NoeudType
    {
        CONTENU,
        STRUCTURE
    }
}
