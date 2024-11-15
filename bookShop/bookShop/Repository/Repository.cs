using Dapper;
using server.Services;
using System.Data;
using System.Data.SqlClient;

namespace server {

    public class Repository {

        private readonly string connectionString;

        public Repository(ISettingsProvider settingsProvider) {
            connectionString = settingsProvider.GetValue<string>("MainDb");
        }

        protected IDbConnection GetConnection() {
            return new SqlConnection(connectionString);
        }

        public BookDto GetBookById(int bookId) {

            BookDto foundBook = null;
            GetConnection().Query<BookDto, AuthorDto, GenreDto, BookDto>(@"SELECT b.BOOK_ID as id, b.NAME as Title, b.PRICE, b.PUBLISHER, b.DESCRIPTION,
       A.AUTHOR_ID as Id, A.NAME,
       G.GENRE_ID as Id, G.NAME
FROM Books b
LEFT JOIN dbo.BookAuthor BA on b.BOOK_ID = BA.BOOK_ID
LEFT JOIN dbo.Authors A on A.AUTHOR_ID = BA.AUTHOR_ID
LEFT JOIN dbo.BookGenre BG on b.BOOK_ID = BG.BOOK_ID
LEFT JOIN dbo.Genres G on bg.GENRE_ID = G.GENRE_ID
WHERE b.BOOK_ID = @bookId",
                ((book, author, genre) => {
                    if (foundBook == null) {
                        foundBook = book;
                        foundBook.Authors = new List<AuthorDto>();
                        foundBook.Genres= new List<GenreDto>();
                    }
                    if (author != null) {
                        if (foundBook.Authors.All(a => a.Id != author.Id))
                            foundBook.Authors.Add(author);
                    }

                    if (genre != null) {
                        if (foundBook.Genres.All(g => g.Id != genre.Id))
                            foundBook.Genres.Add(genre);
                    }
                    return foundBook;
                }),
                new { bookId }, splitOn: "Id");
            return foundBook;
        }

        public IEnumerable<BookDto> GetBooks(int take, int skip) {

            var cache = new List<BookDto>();
            GetConnection().Query<BookDto, AuthorDto, GenreDto, BookDto>(@"with cte(BOOK_ID, NAME, PRICE, PUBLISHER, DESCRIPTION) as (
    SELECT b.BOOK_ID as id, b.NAME as Title, b.PRICE, b.PUBLISHER, b.DESCRIPTION FROM Books b
ORDER BY b.BOOK_ID
OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY
)
SELECT b.BOOK_ID as id, b.NAME as Title, b.PRICE, b.PUBLISHER, b.DESCRIPTION,
       A.AUTHOR_ID as Id, A.NAME,
       G.GENRE_ID as Id, G.NAME
FROM cte b
LEFT JOIN dbo.BookAuthor BA on b.BOOK_ID = BA.BOOK_ID
LEFT JOIN dbo.Authors A on A.AUTHOR_ID = BA.AUTHOR_ID
LEFT JOIN dbo.BookGenre BG on b.BOOK_ID = BG.BOOK_ID
LEFT JOIN dbo.Genres G on bg.GENRE_ID = G.GENRE_ID",
                ((book, author, genre) => {
                    var foundBook = cache.FirstOrDefault(b => b.Id == book.Id);
                    if (foundBook == null) {
                        foundBook = book;
                        foundBook.Authors = new List<AuthorDto>();
                        foundBook.Genres= new List<GenreDto>();
                        cache.Add(foundBook);
                    }
                    if (author != null) {
                        if (foundBook.Authors.All(a => a.Id != author.Id))
                            foundBook.Authors.Add(author);
                    }

                    if (genre != null) {
                        if (foundBook.Genres.All(g => g.Id != genre.Id))
                            foundBook.Genres.Add(genre);
                    }
                    return foundBook;
                }),
                new { skip, take }, splitOn: "Id");
            return cache;
        }

        public int UpdateBookDescriptionsByGenre(string genreName) {
            var updatedRows = GetConnection().Execute("UpdateBookDescriptionsByGenre", new { genreName }, commandType: CommandType.StoredProcedure);
            return updatedRows;
        }

        public int GetAuthorsCountByRegion(string region) {
            var result = GetConnection().ExecuteScalar<int>("Select dbo.GetAuthorsCountByRegion(@region)", new { region });
            return result;
        }

        public IEnumerable<FuncDto> GetEverySecondBookBySalesCount(int value) {
            var result = GetConnection().Query<FuncDto>("SELECT * FROM dbo.GetEverySecondBookBySalesCount(@value)", new { value });
            return result;
        }

        public int AddBookAuthor(int bookId, int authorId) {
            var rows = GetConnection().Execute("INSERT INTO BookAuthor (BOOK_ID, AUTHOR_ID) VALUES (@bookId, @authorId)", new { bookId, authorId });
            return rows;
        }
    }
}
