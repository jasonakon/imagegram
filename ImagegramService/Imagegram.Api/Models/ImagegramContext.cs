using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Imagegram.Api.Models
{
    public partial class ImagegramContext : DbContext
    {
        public ImagegramContext()
        {
        }

        public ImagegramContext(DbContextOptions<ImagegramContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.HasIndex(e => e.ImageId, "image_id");

                entity.Property(e => e.CommentId).HasColumnName("comment_id");

                entity.Property(e => e.Content)
                    .HasMaxLength(50)
                    .HasColumnName("content");

                entity.Property(e => e.CreatedTimestamp).HasColumnName("created_timestamp");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK__Image");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.CreatedTimestamp).HasColumnName("created_timestamp");

                entity.Property(e => e.ModifiedTimestamp).HasColumnName("modified_timestamp");

                entity.Property(e => e.NumComments).HasColumnName("num_comments");

                entity.Property(e => e.Url)
                    .HasMaxLength(50)
                    .HasColumnName("url");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
