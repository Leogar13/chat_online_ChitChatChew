namespace ChitChatChew.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ContactTicket
    {
        public int id { get; set; }

        [Required]
        public string senderName { get; set; }

        [Required]
        public string senderEmail { get; set; }

        [Required]
        public string senderPhone { get; set; }

        [Required]
        public string ticketText { get; set; }
    }
}
