using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickNote_Data;
using QuickNote_Data.Entities;
using QuickNote_Models.Note;

namespace QuickNote_Services.Note
{
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _db;
        private readonly int _userId;

        public NoteService(ApplicationDbContext db, int userId)
        {
            _db = db;
            _userId = userId;
        }

        public async Task<bool> CreateNoteAsync(NoteCreate model)
        {
            var note = new NoteEntity
            {
                OwnerId = _userId,
                Title = model.Title,
                Content = model.Content,
                CreatedUtc = DateTimeOffset.Now
            };

            await _db.Notes.AddAsync(note);
            
            return await _db.SaveChangesAsync() == 1;
        }

        public IEnumerable<NoteListItem> GetAllNotes()
        {
            var query = _db.Notes.Where(note => note.OwnerId == _userId)
                                .Select(note => new NoteListItem
                                {
                                    Id = note.Id,
                                    Title = note.Title,
                                    CreatedUtc = note.CreatedUtc
                                });
            
            return query.ToList();
        }
    }
}