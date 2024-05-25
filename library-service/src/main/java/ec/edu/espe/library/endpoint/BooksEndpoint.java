package ec.edu.espe.library.endpoint;

import ec.edu.espe.library.entity.Author;
import ec.edu.espe.library.entity.Book;
import ec.edu.espe.library.repository.BookRepository;
import ec.edu.espe.library.service.BookService;
import ec.edu.espe.library.xjc.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.ws.server.endpoint.annotation.Endpoint;
import org.springframework.ws.server.endpoint.annotation.PayloadRoot;
import org.springframework.ws.server.endpoint.annotation.RequestPayload;
import org.springframework.ws.server.endpoint.annotation.ResponsePayload;

import javax.xml.datatype.DatatypeFactory;
import javax.xml.datatype.XMLGregorianCalendar;
import java.io.IOException;
import java.util.List;

@Endpoint
public class BooksEndpoint {
    private static final String NAMESPACE_URI = "https://www.espe.edu.ec/library";
    private final BookService bookService;

    @Autowired
    public BooksEndpoint(BookService bookService) {
        this.bookService = bookService;
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "getBooksRequest")
    @ResponsePayload
    public GetBooksResponse getBooks(@RequestPayload GetBooksRequest request) {
        return bookService.getAllBooks();
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "getBookRequest")
    @ResponsePayload
    public GetBookResponse getBooks(@RequestPayload GetBookRequest request) {
        return bookService.getBook(request);
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "addBookRequest")
    @ResponsePayload
    public AddBookResponse addBook(@RequestPayload AddBookRequest request) {
        return bookService.addBook(request);
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "updateBookRequest")
    @ResponsePayload
    public UpdateBookResponse updateBook(@RequestPayload UpdateBookRequest request) {
        return bookService.updateBook(request);
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "deleteBookRequest")
    @ResponsePayload
    public DeleteBookResponse deleteBook(@RequestPayload DeleteBookRequest request) {
        return bookService.deleteBook(request);
    }
}
