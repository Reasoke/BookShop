using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace server.Controllers {

    [ApiController]
    public class BooksController : ControllerBase {
        private readonly Repository repository;

        public BooksController(Repository repository) {
            this.repository = repository;
        }

        [HttpGet, Route("api/books")]
        public IEnumerable<BookDto> Get(int take = 100, int skip = 0) {

            if (take < 1)
                throw new ArgumentException("Take is too small, 1 is a min.");
            if (take > 1000)
                throw new ArgumentException("Take is too big, 100 is a max.");

            var books = repository.GetBooks(take, skip);
            return books;
        }

        [HttpPost, Route("api/books/updateDesc")] //todo: post
        public IActionResult UpdateBookDescriptionsByGenre(string genreName) {

            if (string.IsNullOrEmpty(genreName))
                return BadRequest();

            var updatedRows = repository.UpdateBookDescriptionsByGenre(genreName);
            return Ok(new {
                BooksUpdated = updatedRows,
            });
        }

        [HttpGet, Route("api/books/authorsCountByRegion")]
        public IActionResult GetAuthorsCountByRegion(string region) {

            if (string.IsNullOrEmpty(region))
                return BadRequest();

            var scalarResult = repository.GetAuthorsCountByRegion(region);
            return Ok(new {
                AuthorsCountByRegion = scalarResult,
            });
        }

        [HttpGet, Route("api/books/everySecondBookBySalesCount")]
        public IActionResult GetEverySecondBookBySalesCount(int value) {

            if (value < 0)
                return BadRequest();

            var result = repository.GetEverySecondBookBySalesCount(value);
            return Ok(result);
        }


        [HttpPost, Route("api/books/{bookId:int}/addAuthor")] //todo: post
        public IActionResult AddBookAuthor(int bookId, int authorId) {
            if (bookId < 0)
                return BadRequest();
            if (authorId < 0)
                return BadRequest();

            try {
                var book = repository.GetBookById(bookId);
                if (book == null)
                    return new NotFoundResult();

                var rows = repository.AddBookAuthor(bookId, authorId);
                return Ok(new {
                    AuthorsAdded = rows,
                });
            }
            catch (Exception ex) {
                return new ObjectResult(new { 
                    //type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                    title = "Internal server error",
                    status = 503,
                    error = ex.Message,
                }) { StatusCode = 403 };
            }
        }
    }
}
