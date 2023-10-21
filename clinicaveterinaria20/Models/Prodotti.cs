namespace clinicaveterinaria20.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Prodotti")]
    public partial class Prodotti
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prodotti()
        {
            Vendita = new HashSet<Vendita>();
        }

        [Key]
        public int idprodotto { get; set; }

        [Required(ErrorMessage = "campo richiesto")]
        [StringLength(200)]
        [Display(Name = "Inserisci il nome del Prodotto")]
        public string nome { get; set; }

        [Required(ErrorMessage = "campo richiesto")]
        [Display(Name = "Inserisci la tipologia del Prodotto")]
        public bool? tipologia { get; set; }

        [StringLength(200)]
        [Display(Name = "Inserisci un'immagine del Prodotto")]
        public string foto { get; set; }

        [Required(ErrorMessage = "campo richiesto")]
        [Display(Name = "Inserisci la quantit� del Prodotto")]
        public int? quantita { get; set; }

        [Required(ErrorMessage = "campo richiesto")]
        [Display(Name = "Inserisci il prezzo del Prodotto")]
        public decimal? costo { get; set; }
        [Display(Name = "Inserisci un cassetto")]
        public int? idcassetto { get; set; }

        [Display(Name = "Specifica disponibilit�")]
        public bool invendita { get; set; }
        [Display(Name ="Specifica Utilizzo")]
        public int? idutilizzo { get; set; }
        [Display(Name = "Inserisci l'azienda")]
        public int? idbrand { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual Cassetto Cassetto { get; set; }

        public virtual Utilizzi Utilizzi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vendita> Vendita { get; set; }
    }
}