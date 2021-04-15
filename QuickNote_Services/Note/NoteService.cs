using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickNote_Data;
using QuickNote_Data.Entities;
using QuickNote_Models.Note;

namespace QuickNote_Services.Note
{
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _db;
        private readonly int _userId;

        public NoteService(ApplicationDbContext db)
        {
            _db = db;
            _userId = db.GetUserId();
        }

        // public NoteService(ApplicationDbContext db, int userId)
        // {
        //     _db = db;
        //     _userId = userId;
        // }
        
        // Only inject DbContextOptions into ApplicationDbContext
        // public NoteService(DbContextOptions<ApplicationDbContext> options, int userId)
        // {
        //     _options = options;
        //     _userId = userId;
        // }

        public async Task<bool> CreateNoteAsync(NoteCreate model)
        {
            var note = new NoteEntity
            {
                OwnerId = _userId,
                Title = model.Title,
                Content = model.Content,
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // using(var db = new ApplicationDbContext(_options))
            // {
            //     await db.Notes.AddAsync(note);
            
            //     return await db.SaveChangesAsync() == 1;
            // }

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

        public async Task<NoteDetail> GetNoteByNoteIdAsync(int noteId)
        {
            var noteEntity = await _db.Notes.FindAsync(noteId);

            if(noteEntity is null)
                return null;

            return new NoteDetail
            {
                Id = noteEntity.Id,
                Title = noteEntity.Title,
                Content = noteEntity.Content,
                CreatedUtc = noteEntity.CreatedUtc,
                ModifiedUtc = noteEntity.ModifiedUtc
            };
        }

        public async Task<bool> UpdateNoteAsync(NoteEdit updatedNote)
        {
            var originalNote = await _db.Notes.FindAsync(updatedNote.Id);

            if(originalNote == null || originalNote.OwnerId != _userId)
                return false;

            originalNote.Title = updatedNote.Title;
            originalNote.Content = updatedNote.Content;
            originalNote.ModifiedUtc = DateTimeOffset.UtcNow;

            return await _db.SaveChangesAsync() == 1;
        }

        public async Task<bool> DeleteNoteByNoteIdAsync(int noteId)
        {
            var note = await _db.Notes.FindAsync(noteId);

            if(note == null || note.OwnerId != _userId)
                return false;
            
            _db.Notes.Remove(note);

            return await _db.SaveChangesAsync() == 1;
        }
    }
}

