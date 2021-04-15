using System.Collections.Generic;
using System.Threading.Tasks;
using QuickNote_Models.Note;

namespace QuickNote_Services.Note
{
    public interface INoteService
    {
        Task<bool> CreateNoteAsync(NoteCreate model);

        IEnumerable<NoteListItem> GetAllNotes();

        Task<NoteDetail> GetNoteByNoteIdAsync(int noteId);
    }
}