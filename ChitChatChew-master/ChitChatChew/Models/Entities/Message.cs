namespace ChitChatChew.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Message
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Message()
        {
            Groups = new HashSet<Group>();
        }

        public int id { get; set; }

        public int senderId { get; set; }

        public string messageText { get; set; }

        public int? messageImg { get; set; }

        public int? messageGif { get; set; }

        public int? messageFile { get; set; }

        public int? messageSticker { get; set; }

        public DateTime? createdAt { get; set; }

        public virtual File File { get; set; }

        public virtual Gif Gif { get; set; }

        public virtual Image Image { get; set; }

        public virtual Sticker Sticker { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Group> Groups { get; set; }
    }
}
