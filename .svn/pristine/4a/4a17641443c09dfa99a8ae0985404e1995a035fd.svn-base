namespace ChitChatChew.Models.Entities
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MainModels : DbContext
    {
        public MainModels()
            : base("name=MainModels")
        {
        }

        public virtual DbSet<ContactTicket> ContactTickets { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Gif> Gifs { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Sticker> Stickers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactTicket>()
                .Property(e => e.senderEmail)
                .IsUnicode(false);

            modelBuilder.Entity<ContactTicket>()
                .Property(e => e.senderPhone)
                .IsUnicode(false);

            modelBuilder.Entity<File>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.File)
                .HasForeignKey(e => e.messageFile);

            modelBuilder.Entity<Gif>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.Gif)
                .HasForeignKey(e => e.messageGif);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.Messages)
                .WithMany(e => e.Groups)
                .Map(m => m.ToTable("GroupMessages").MapLeftKey("groupId").MapRightKey("messageId"));

            modelBuilder.Entity<Group>()
                .HasMany(e => e.Users)
                .WithMany(e => e.InGroups)
                .Map(m => m.ToTable("GroupUsers").MapLeftKey("groupId").MapRightKey("userId"));

            modelBuilder.Entity<Image>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.Image)
                .HasForeignKey(e => e.messageImg);

            modelBuilder.Entity<Sticker>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.Sticker)
                .HasForeignKey(e => e.messageSticker);

            modelBuilder.Entity<User>()
                .Property(e => e.pw)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.verificationCode)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.statusText)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Files)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.createdBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Groups)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.createdBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Images)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.createdBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.senderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.OtherBlockList)
                .WithMany(e => e.BlockList)
                .Map(m => m.ToTable("UserBlocks").MapLeftKey("blockedId").MapRightKey("id"));

            modelBuilder.Entity<User>()
                .HasMany(e => e.OtherFriendList)
                .WithMany(e => e.FriendList)
                .Map(m => m.ToTable("UserFriends").MapLeftKey("friendId").MapRightKey("id"));
        }
    }
}
