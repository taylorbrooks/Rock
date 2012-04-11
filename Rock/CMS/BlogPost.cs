//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the T4\Model.tt template.
//
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;

using Rock.Data;

namespace Rock.CMS
{
    /// <summary>
    /// Blog Post POCO Entity.
    /// </summary>
    [Table( "cmsBlogPost" )]
    public partial class BlogPost : ModelWithAttributes<BlogPost>, IAuditable
    {
		/// <summary>
		/// Gets or sets the Blog Id.
		/// </summary>
		/// <value>
		/// Blog Id.
		/// </value>
		[DataMember]
		public int BlogId { get; set; }
		
		/// <summary>
		/// Gets or sets the Title.
		/// </summary>
		/// <value>
		/// Title.
		/// </value>
		[MaxLength( 250 )]
		[DataMember]
		public string Title { get; set; }
		
		/// <summary>
		/// Gets or sets the Content.
		/// </summary>
		/// <value>
		/// Content.
		/// </value>
		[DataMember]
		public string Content { get; set; }
		
		/// <summary>
		/// Gets or sets the Author Id.
		/// </summary>
		/// <value>
		/// Author Id.
		/// </value>
		[DataMember]
		public int? AuthorId { get; set; }
		
		/// <summary>
		/// Gets or sets the State.
		/// </summary>
		/// <value>
		/// State.
		/// </value>
		[DataMember]
		public int State { get; set; }
		
		/// <summary>
		/// Gets or sets the Publish Date.
		/// </summary>
		/// <value>
		/// Publish Date.
		/// </value>
		[DataMember]
		public DateTime? PublishDate { get; set; }
		
		/// <summary>
		/// Gets or sets the Created Date Time.
		/// </summary>
		/// <value>
		/// Created Date Time.
		/// </value>
		[DataMember]
		public DateTime? CreatedDateTime { get; set; }
		
		/// <summary>
		/// Gets or sets the Modified Date Time.
		/// </summary>
		/// <value>
		/// Modified Date Time.
		/// </value>
		[DataMember]
		public DateTime? ModifiedDateTime { get; set; }
		
		/// <summary>
		/// Gets or sets the Created By Person Id.
		/// </summary>
		/// <value>
		/// Created By Person Id.
		/// </value>
		[DataMember]
		public int? CreatedByPersonId { get; set; }
		
		/// <summary>
		/// Gets or sets the Modified By Person Id.
		/// </summary>
		/// <value>
		/// Modified By Person Id.
		/// </value>
		[DataMember]
		public int? ModifiedByPersonId { get; set; }
		
		/// <summary>
        /// Gets a Data Transfer Object (lightweight) version of this object.
        /// </summary>
        /// <value>
        /// A <see cref="Rock.CMS.DTO.BlogPost"/> object.
        /// </value>
		public Rock.CMS.DTO.BlogPost DataTransferObject
		{
			get 
			{ 
				Rock.CMS.DTO.BlogPost dto = new Rock.CMS.DTO.BlogPost();
				dto.Id = this.Id;
				dto.Guid = this.Guid;
				dto.BlogId = this.BlogId;
				dto.Title = this.Title;
				dto.Content = this.Content;
				dto.AuthorId = this.AuthorId;
				dto.State = this.State;
				dto.PublishDate = this.PublishDate;
				dto.CreatedDateTime = this.CreatedDateTime;
				dto.ModifiedDateTime = this.ModifiedDateTime;
				dto.CreatedByPersonId = this.CreatedByPersonId;
				dto.ModifiedByPersonId = this.ModifiedByPersonId;
				return dto; 
			}
		}

        /// <summary>
        /// Gets the auth entity.
        /// </summary>
		[NotMapped]
		public override string AuthEntity { get { return "CMS.BlogPost"; } }
        
		/// <summary>
        /// Gets or sets the Blog Categories.
        /// </summary>
        /// <value>
        /// Collection of Blog Categories.
        /// </value>
		public virtual ICollection<BlogCategory> BlogCategories { get; set; }
        
		/// <summary>
        /// Gets or sets the Blog Post Comments.
        /// </summary>
        /// <value>
        /// Collection of Blog Post Comments.
        /// </value>
		public virtual ICollection<BlogPostComment> BlogPostComments { get; set; }
        
		/// <summary>
        /// Gets or sets the Blog Tags.
        /// </summary>
        /// <value>
        /// Collection of Blog Tags.
        /// </value>
		public virtual ICollection<BlogTag> BlogTags { get; set; }
        
		/// <summary>
        /// Gets or sets the Blog.
        /// </summary>
        /// <value>
        /// A <see cref="Blog"/> object.
        /// </value>
		public virtual Blog Blog { get; set; }
        
		/// <summary>
        /// Gets or sets the Author.
        /// </summary>
        /// <value>
        /// A <see cref="CRM.Person"/> object.
        /// </value>
		public virtual CRM.Person Author { get; set; }
        
		/// <summary>
        /// Gets or sets the Created By Person.
        /// </summary>
        /// <value>
        /// A <see cref="CRM.Person"/> object.
        /// </value>
		public virtual CRM.Person CreatedByPerson { get; set; }
        
		/// <summary>
        /// Gets or sets the Modified By Person.
        /// </summary>
        /// <value>
        /// A <see cref="CRM.Person"/> object.
        /// </value>
		public virtual CRM.Person ModifiedByPerson { get; set; }

    }
    /// <summary>
    /// Blog Post Configuration class.
    /// </summary>
    public partial class BlogPostConfiguration : EntityTypeConfiguration<BlogPost>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlogPostConfiguration"/> class.
        /// </summary>
        public BlogPostConfiguration()
        {
            this.HasMany( p => p.BlogTags ).WithMany( c => c.BlogPosts ).Map( m => { m.MapLeftKey("BlogTagId"); m.MapRightKey("BlogPostId"); m.ToTable("cmsBlogPostTag" ); } );
			this.HasRequired( p => p.Blog ).WithMany( p => p.BlogPosts ).HasForeignKey( p => p.BlogId ).WillCascadeOnDelete(false);
			this.HasOptional( p => p.Author ).WithMany( p => p.BlogPosts ).HasForeignKey( p => p.AuthorId ).WillCascadeOnDelete(false);
			this.HasOptional( p => p.CreatedByPerson ).WithMany().HasForeignKey( p => p.CreatedByPersonId ).WillCascadeOnDelete(false);
			this.HasOptional( p => p.ModifiedByPerson ).WithMany().HasForeignKey( p => p.ModifiedByPersonId ).WillCascadeOnDelete(false);
		}
    }
}
