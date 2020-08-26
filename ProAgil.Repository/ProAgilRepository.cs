using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public class ProAgilRepository : IProAgilRepository
    {
        public ProAgilContext _Context { get; }
        public ProAgilRepository(ProAgilContext context)
        {
            _Context = context;

        }
        //GERAIS
        public void Add<T>(T entity) where T : class
        {
            _Context.Add(entity);
        }
        public void Update<T>(T entity) where T : class
        {
            _Context.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _Context.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _Context.SaveChangesAsync()) > 0;
        }

        //EVENTO    
        public async Task<Evento[]> GetAllEventoAsync(bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _Context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);
            
            if(includePalestrantes){
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }

            query = query.OrderByDescending(c => c.DataEvento);

            return await query.ToArrayAsync();
        }
        public async Task<Evento[]> GetAllEventoAsyncByTema(string tema, bool includePalestrantes)
        {
            IQueryable<Evento> query = _Context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);
            
            if(includePalestrantes){
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }

            query = query.OrderByDescending(c => c.DataEvento)
                        .Where(c => c.Tema.ToLower().Contains(tema.ToLower()));

            return await query.ToArrayAsync();
        }

        public async Task<Evento> GetEventoAsyncById(int EventoId, bool includePalestrantes)
        {
            IQueryable<Evento> query = _Context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);
            
            if(includePalestrantes){
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }

            query = query.OrderByDescending(c => c.DataEvento)
                        .Where(c => c.Id == EventoId);

            return await query.FirstOrDefaultAsync();
        }

        //PALESTRANTE
        public async Task<Palestrante> GetPalestranteAsync(int PalestranteId,bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _Context.Palestrantes
                .Include(c => c.RedesSociais);
            
            if(includeEventos){
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);
            }

            query = query.OrderBy(p => p.Nome)
                .Where(p => p.Id == PalestranteId);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Palestrante[]> GetAllPalestrateAsyncByName(string name, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _Context.Palestrantes
                .Include(c => c.RedesSociais);
            
            if(includeEventos){
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);
            }

            query = query.Where(p => p.Nome.ToLower().Contains(name.ToLower()))

            return await query.ToArrayAsync();
        }

    }
}