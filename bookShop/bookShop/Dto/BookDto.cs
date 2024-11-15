namespace server {

    public class AuthorDto {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GenreDto {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    //public class AuthorDetailsDto {
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Address { get; set; }
    //    public string Information { get; set; }
    //}

    public class BookDto {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Publisher { get; set; }
        public string Description { get; set; }

        public List<AuthorDto> Authors { get; set; }
        public List<GenreDto> Genres { get; set; }
    }

    public class FuncDto {
        public string NAME { get; set; }
        public int SALE_QUANTITY { get; set; }
        public string PUBLISHER { get; set; }
    }

}
