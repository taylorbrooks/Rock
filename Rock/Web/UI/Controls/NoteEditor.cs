﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls
{
    /// <summary>
    /// Edits a Rock Note.
    /// </summary>
    [ToolboxData( "<{0}:NoteEditor runat=server></{0}:NoteEditor>" )]
    public class NoteEditor : CompositeControl
    {
        #region Fields

        private DropDownList _ddlNoteType;
        private RockTextBox _tbNote;
        private CheckBox _cbAlert;
        private CheckBox _cbPrivate;
        private LinkButton _lbSaveNote;

        // NOTE: Intentially using a HtmlAnchor for security instead of SecurityButton since the URL will need to be set in Javascript
        private HtmlAnchor _aSecurity;

        private DateTimePicker _dtCreateDate;
        private HiddenFieldWithClass _hfParentNoteId;
        private HiddenFieldWithClass _hfNoteId;
        private ModalAlert _mdEditWarning;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the note options.
        /// </summary>
        /// <value>
        /// The note options.
        /// </value>
        public NoteOptions NoteOptions { get; private set; }

        /// <summary>
        /// Sets the note.
        /// </summary>
        /// <value>
        /// The note.
        /// </value>
        public Rock.Model.Note Note
        {
            set
            {
                EnsureChildControls();
                this.NoteId = value.Id;
                this.NoteTypeId = value.NoteTypeId;
                this.EntityId = value.EntityId;
                this.CreatedByPersonAlias = value.CreatedByPersonAlias;
                this.Text = value.Text;
                this.IsAlert = value.IsAlert.HasValue && value.IsAlert.Value;
                this.IsPrivate = value.IsPrivateNote;
                this.ParentNoteId = value.ParentNoteId;
            }
        }

        /// <summary>
        /// Gets or sets the note type identifier.
        /// </summary>
        /// <value>
        /// The note type identifier.
        /// </value>
        public int? NoteTypeId
        {
            get
            {
                int? noteTypeId = ViewState["NoteTypeId"] as int?;
                if ( !noteTypeId.HasValue && NoteOptions.NoteTypes.Any() )
                {
                    noteTypeId = NoteOptions.NoteTypes.First().Id;
                }

                return noteTypeId ?? 0;
            }

            set
            {
                ViewState["NoteTypeId"] = value;

                EnsureChildControls();
                if ( value.HasValue )
                {
                    _ddlNoteType.SetValue( value.ToString() );
                }
                else
                {
                    _ddlNoteType.SelectedIndex = -1;
                }
            }
        }

        /// <summary>
        /// Gets or sets the note id.
        /// </summary>
        /// <value>
        /// The note id.
        /// </value>
        public int? NoteId
        {
            get
            {
                return _hfNoteId.Value.AsIntegerOrNull();
            }

            set
            {
                _hfNoteId.Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        public int? EntityId
        {
            get { return ViewState["EntityId"] as int?; }
            set { ViewState["EntityId"] = value; }
        }

        /// <summary>
        /// Gets or sets the created by photo identifier.
        /// </summary>
        /// <value>
        /// The created by photo identifier.
        /// </value>
        public int? CreatedByPhotoId
        {
            get { return ViewState["CreatedByPhotoId"] as int?; }
            set { ViewState["CreatedByPhotoId"] = value; }
        }

        /// <summary>
        /// Gets or sets the created by person identifier.
        /// </summary>
        /// <value>
        /// The created by person identifier.
        /// </value>
        public int? CreatedByPersonId
        {
            get { return ViewState["CreatedByPersonId"] as int?; }
            set { ViewState["CreatedByPersonId"] = value; }
        }

        /// <summary>
        /// Gets or sets the created by gender.
        /// </summary>
        /// <value>
        /// The created by gender.
        /// </value>
        public Gender CreatedByGender
        {
            get
            {
                object gender = this.ViewState["Gender"];
                return gender != null ? ( Gender ) gender : Gender.Male;
            }

            set
            {
                ViewState["Gender"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the label for the note entry box
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label
        {
            get
            {
                return ViewState["Label"] as string ?? "Note";
            }

            set
            {
                ViewState["Label"] = value;
                if ( value != null )
                {
                    _tbNote.Placeholder = string.Format( "Write a {0}...", value.ToLower() );
                }
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get
            {
                return _tbNote.Text;
            }

            set
            {
                _tbNote.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is alert.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alert; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlert
        {
            get
            {
                return _cbAlert.Checked;
            }

            set
            {
                _cbAlert.Checked = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is private.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is private; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrivate
        {
            get
            {
                return _cbPrivate.Checked;
            }

            set
            {
                _cbPrivate.Checked = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent note identifier.
        /// </summary>
        /// <value>
        /// The parent note identifier.
        /// </value>
        public int? ParentNoteId
        {
            get
            {
                return _hfParentNoteId.Value.AsIntegerOrNull();
            }

            set
            {
                _hfParentNoteId.Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can edit.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can edit; otherwise, <c>false</c>.
        /// </value>
        public bool CanEdit
        {
            get
            {
                return ViewState["CanEdit"] as bool? ?? false;
            }

            set
            {
                ViewState["CanEdit"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether start this note in Edit mode instead of View mode
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show edit]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowEditMode
        {
            get
            {
                return ViewState["ShowEditMode"] as bool? ?? false;
            }

            set
            {
                ViewState["ShowEditMode"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the created by person alias.
        /// </summary>
        /// <value>
        /// The created by person alias.
        /// </value>
        public PersonAlias CreatedByPersonAlias
        {
            get
            {
                return _createdByPersonAlias;
            }

            set
            {
                _createdByPersonAlias = value;
                this.CreatedByPhotoId = _createdByPersonAlias?.Person?.PhotoId;
                this.CreatedByGender = _createdByPersonAlias?.Person?.Gender ?? Gender.Male;
                this.CreatedByPersonId = _createdByPersonAlias?.Person?.Id;
            }
        }

        private PersonAlias _createdByPersonAlias = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteEditor"/> class.
        /// </summary>
        /// <param name="noteOptions">The note options.</param>
        public NoteEditor( NoteOptions noteOptions )
        {
            this.NoteOptions = noteOptions;
            _ddlNoteType = new DropDownList();

            _tbNote = new RockTextBox();
            _hfNoteId = new HiddenFieldWithClass();
            _tbNote.Placeholder = "Write a note...";
            _cbAlert = new CheckBox();
            _cbPrivate = new CheckBox();
            _lbSaveNote = new LinkButton();
            _aSecurity = new HtmlAnchor();
            _dtCreateDate = new DateTimePicker();
            _hfParentNoteId = new HiddenFieldWithClass();
            _mdEditWarning = new ModalAlert();
        }

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( Page.IsPostBack )
            {
                if ( CanEdit && _ddlNoteType.Visible )
                {
                    NoteTypeId = _ddlNoteType.SelectedValueAsInt();
                }
            }
            else
            {
                var editableNoteTypes = this.NoteOptions.GetEditableNoteTypes( ( this.Page as RockPage )?.CurrentPerson );
                _ddlNoteType.DataSource = editableNoteTypes;
                _ddlNoteType.DataBind();
                _ddlNoteType.Visible = editableNoteTypes.Count() > 1;
            }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _ddlNoteType.ID = this.ID + "_ddlNoteType";
            _ddlNoteType.CssClass = "form-control input-sm input-width-lg noteentry-notetype js-notenotetype";
            _ddlNoteType.DataValueField = "Id";
            _ddlNoteType.DataTextField = "Name";
            Controls.Add( _ddlNoteType );

            _hfNoteId.ID = this.ID + "_hfNoteId";
            _hfNoteId.CssClass = "js-noteid";
            Controls.Add( _hfNoteId );

            _hfParentNoteId.ID = this.ID + "_hfParentNoteId";
            _hfParentNoteId.CssClass = "js-parentnoteid";
            Controls.Add( _hfParentNoteId );

            _tbNote.ID = this.ID + "_tbNewNote";
            _tbNote.TextMode = TextBoxMode.MultiLine;
            _tbNote.CssClass = "js-notetext";
            _tbNote.ValidateRequestMode = ValidateRequestMode.Disabled;
            Controls.Add( _tbNote );

            _cbAlert.ID = this.ID + "_cbAlert";
            _cbAlert.Text = "Alert";
            _cbAlert.CssClass = "js-notealert";
            Controls.Add( _cbAlert );

            _cbPrivate.ID = this.ID + "_cbPrivate";
            _cbPrivate.Text = "Private";
            _cbPrivate.CssClass = "js-noteprivate";
            Controls.Add( _cbPrivate );

            _mdEditWarning.ID = this.ID + "_mdEditWarning";
            Controls.Add( _mdEditWarning );

            _lbSaveNote.ID = this.ID + "_lbSaveNote";
            _lbSaveNote.Attributes["class"] = "btn btn-primary btn-xs";
            _lbSaveNote.CausesValidation = false;
            _lbSaveNote.Click += lbSaveNote_Click;

            Controls.Add( _lbSaveNote );

            _aSecurity.ID = "_aSecurity";
            _aSecurity.Attributes["class"] = "btn btn-security btn-xs security pull-right fa fa-lock js-notesecurity";
            _aSecurity.Attributes["data-entitytype-id"] = EntityTypeCache.Read( typeof( Rock.Model.Note ) ).Id.ToString();
            Controls.Add( _aSecurity );

            _dtCreateDate.ID = this.ID + "_tbCreateDate";
            _dtCreateDate.Label = "Note Created Date";
            Controls.Add( _dtCreateDate );
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        public override void RenderControl( HtmlTextWriter writer )
        {
            var noteType = NoteTypeId.HasValue ? NoteTypeCache.Read( NoteTypeId.Value ) : null;
            StringBuilder noteCss = new StringBuilder();
            noteCss.Append( "note js-note-editor" );
            if ( !string.IsNullOrEmpty( noteType?.CssClass ) )
            {
                noteCss.Append( " " + noteType.CssClass );
            }

            if ( !string.IsNullOrEmpty( this.CssClass ) )
            {
                noteCss.Append( " " + this.CssClass );
            }

            writer.AddAttribute( HtmlTextWriterAttribute.Class, noteCss.ToString() );
            if ( this.Style[HtmlTextWriterStyle.Display] != null )
            {
                writer.AddStyleAttribute( HtmlTextWriterStyle.Display, this.Style[HtmlTextWriterStyle.Display] );
            }

            if ( !ShowEditMode )
            {
                writer.AddStyleAttribute( HtmlTextWriterStyle.Display, "none" );
            }

            if ( this.NoteId.HasValue )
            {
                writer.AddAttribute( "rel", this.NoteId.Value.ToStringSafe() );
            }

            writer.RenderBeginTag( HtmlTextWriterTag.Div );

            // Edit Mode HTML...
            writer.AddAttribute( HtmlTextWriterAttribute.Class, "panel panel-noteentry" );

            writer.RenderBeginTag( HtmlTextWriterTag.Div );

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "panel-body" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );

            if ( NoteOptions.DisplayType == NoteDisplayType.Full && NoteOptions.UsePersonIcon )
            {
                writer.Write( Person.GetPersonPhotoImageTag( CreatedByPersonId, CreatedByPhotoId, null, CreatedByGender, null, 50, 50 ) );
            }

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "noteentry-control" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            _ddlNoteType.RenderControl( writer );
            _tbNote.RenderControl( writer );
            writer.RenderEndTag();

            _hfNoteId.RenderControl( writer );
            _hfParentNoteId.RenderControl( writer );

            // The optional create date text box, but only for new notes...
            if ( NoteOptions.ShowCreateDateInput && !NoteId.HasValue )
            {
                writer.AddAttribute( HtmlTextWriterAttribute.Class, "createDate clearfix" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );
                _dtCreateDate.RenderControl( writer );
                writer.RenderEndTag();  // createDate div
            }

            if ( NoteOptions.DisplayType == NoteDisplayType.Full )
            {
                // Options
                writer.AddAttribute( HtmlTextWriterAttribute.Class, "settings clearfix" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );

                writer.AddAttribute( HtmlTextWriterAttribute.Class, "options pull-left" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );

                if ( NoteOptions.ShowAlertCheckBox )
                {
                    _cbAlert.RenderControl( writer );
                }

                if ( NoteOptions.ShowPrivateCheckBox )
                {
                    _cbPrivate.RenderControl( writer );
                }

                writer.RenderEndTag();

                if ( NoteOptions.ShowSecurityButton )
                {
                    _aSecurity.Attributes["data-title"] = this.Label;
                    _aSecurity.RenderControl( writer );
                }

                writer.RenderEndTag();  // settings div
            }

            writer.RenderEndTag();  // panel body

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "panel-footer" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );

            _mdEditWarning.RenderControl( writer );

            _lbSaveNote.Text = "Save " + Label;
            _lbSaveNote.CommandName = "SaveNote";
            _lbSaveNote.CommandArgument = this.NoteId.ToString();
            _lbSaveNote.RenderControl( writer );

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "edit-note-cancel js-editnote-cancel btn btn-link btn-xs" );
            writer.RenderBeginTag( HtmlTextWriterTag.A );
            writer.Write( "Cancel" );
            writer.RenderEndTag();

            writer.RenderEndTag();  // panel-footer div

            writer.RenderEndTag();  // note-entry div

            writer.RenderEndTag();
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the lbSaveNote control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbSaveNote_Click( object sender, EventArgs e )
        {
            if ( _ddlNoteType.Visible )
            {
                NoteTypeId = _ddlNoteType.SelectedValueAsInt() ?? 0;
            }

            var rockPage = this.Page as RockPage;
            if ( rockPage != null && NoteTypeId.HasValue )
            {
                var currentPerson = rockPage.CurrentPerson;

                var rockContext = new RockContext();
                var service = new NoteService( rockContext );
                Note note = null;

                if ( NoteId.HasValue )
                {
                    note = service.Get( NoteId.Value );
                }

                if ( note == null )
                {
                    note = new Note();
                    note.IsSystem = false;
                    note.EntityId = EntityId;
                    note.ParentNoteId = _hfParentNoteId.Value.AsIntegerOrNull();
                    service.Add( note );
                }
                else
                {
                    if ( !note.IsAuthorized( Authorization.EDIT, currentPerson ) )
                    {
                        // if somehow a person is trying to edit a note that they aren't authorized to edit, don't update the note
                        _mdEditWarning.Show( "Not authorized to edit note", ModalAlertType.Warning );
                        return;
                    }
                }

                note.NoteTypeId = NoteTypeId.Value;
                if ( string.IsNullOrWhiteSpace( note.Caption ) )
                {
                    note.Caption = IsPrivate ? "You - Personal Note" : string.Empty;
                }

                note.Text = Text;
                note.IsAlert = IsAlert;
                note.IsPrivateNote = IsPrivate;

                if ( NoteOptions.ShowCreateDateInput )
                {
                    note.CreatedDateTime = _dtCreateDate.SelectedDateTime;
                }

                note.EditedByPersonAliasId = currentPerson?.PrimaryAliasId;
                note.EditedDateTime = RockDateTime.Now;
                note.NoteUrl = this.RockBlock()?.CurrentPageReference?.BuildUrl();

                var noteType = NoteTypeCache.Read( note.NoteTypeId );

                if ( noteType.RequiresApprovals )
                {
                    if ( note.IsAuthorized( Authorization.APPROVE, currentPerson ) )
                    {
                        note.ApprovalStatus = NoteApprovalStatus.Approved;
                    }
                    else
                    {
                        note.ApprovalStatus = NoteApprovalStatus.PendingApproval;
                    }
                }
                else
                {
                    note.ApprovalStatus = NoteApprovalStatus.Approved;
                }

                rockContext.SaveChanges();

                if ( SaveButtonClick != null )
                {
                    SaveButtonClick( this, new NoteEventArgs( note.Id ) );
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Occurs when [save button click].
        /// </summary>
        public event EventHandler<NoteEventArgs> SaveButtonClick;

        #endregion
    }

    /// <summary>
    /// Note Event Argument includes id of note updated
    /// </summary>
    public class NoteEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the note identifier.
        /// </summary>
        /// <value>
        /// The note identifier.
        /// </value>
        public int? NoteId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteEventArgs"/> class.
        /// </summary>
        /// <param name="noteId">The note identifier.</param>
        public NoteEventArgs( int? noteId )
        {
            NoteId = noteId;
        }
    }

    #region Enums

    /// <summary>
    /// 
    /// </summary>
    public enum NoteDisplayType
    {
        /// <summary>
        /// The full
        /// </summary>
        Full,

        /// <summary>
        /// The light
        /// </summary>
        Light
    }

    #endregion
}
