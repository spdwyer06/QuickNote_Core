using System;
using System.ComponentModel.DataAnnotations;

namespace QuickNote_Data.Entities
{
    public class NoteEntity
    {
        [Key]
        public int Id { get; set; }

        // Data Relationship was set up in ApplicationDbContext.cs in OnModelCreating()
        // [ForeignKey(nameof(Owner))]
            // FK property
        // public int OwnerId { get; set; }
            // Navigation property
        // public UserEntity Owner { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public UserEntity Owner { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTimeOffset CreatedUtc { get; set; }

        public DateTimeOffset? ModifiedUtc { get; set; }
    }
}