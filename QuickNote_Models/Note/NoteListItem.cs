using System;

namespace QuickNote_Models.Note
{
    public class NoteListItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTimeOffset CreatedUtc { get; set; }
    }
}