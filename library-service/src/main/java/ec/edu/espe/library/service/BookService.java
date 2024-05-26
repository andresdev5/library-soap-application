package ec.edu.espe.library.service;

import ec.edu.espe.library.entity.Author;
import ec.edu.espe.library.entity.Book;
import ec.edu.espe.library.entity.Genre;
import ec.edu.espe.library.repository.AuthorRepository;
import ec.edu.espe.library.repository.BookRepository;
import ec.edu.espe.library.repository.GenreRepository;
import ec.edu.espe.library.util.DateUtil;
import ec.edu.espe.library.xjc.*;
import jakarta.transaction.Transactional;
import org.imgscalr.Scalr;
import org.springframework.stereotype.Service;

import javax.imageio.IIOImage;
import javax.imageio.ImageIO;
import javax.imageio.ImageWriteParam;
import javax.imageio.ImageWriter;
import javax.imageio.stream.FileImageOutputStream;
import javax.xml.datatype.XMLGregorianCalendar;
import java.awt.image.BufferedImage;
import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@Service
public class BookService {
    private final BookRepository bookRepository;
    private final AuthorRepository authorRepository;
    private final GenreRepository genreRepository;

    public BookService(
            BookRepository bookRepository,
            AuthorRepository authorRepository,
            GenreRepository genreRepository) {
        this.bookRepository = bookRepository;
        this.authorRepository = authorRepository;
        this.genreRepository = genreRepository;
    }

    public GetBooksResponse getAllBooks() {
        List<Book> books = bookRepository.findAllByOrderByIdDesc();
        GetBooksResponse response = new GetBooksResponse();

        books.forEach(book -> {
            response.getBook().add(mapBookToBookModel(book));
        });

        return response;
    }

    public GetBookResponse getBook(GetBookRequest request) {
        Optional<Book> book = bookRepository.findById(request.getId());
        GetBookResponse response = new GetBookResponse();
        Book bookFound = book.orElseThrow(() -> new RuntimeException("Book not found"));
        response.setBook(mapBookToBookModel(bookFound));
        return response;
    }

    @Transactional(rollbackOn = Exception.class)
    public AddBookResponse addBook(AddBookRequest request) {
        Long authorId = request.getBook().getAuthor().getId();
        List<Genre> genres = request.getBook().getGenres()
                .stream()
                .map(genreModel -> genreRepository.findById(genreModel.getId())
                    .orElseThrow(() -> new RuntimeException("Genre with id " + genreModel.getId() + "not found"))).toList();
        Author author = authorRepository.findById(authorId)
                .orElseThrow(() -> new RuntimeException("Author not found"));
        Book book = Book.builder()
                .title(request.getBook().getTitle())
                .year(request.getBook().getYear())
                .isbn(request.getBook().getIsbn())
                .publisher(request.getBook().getPublisher())
                .price(request.getBook().getPrice())
                .author(author)
                .genres(genres)
                .build();

        Book saved = bookRepository.save(book);

        if (request.getBook().getImage() != null) {
            try {
                book.setImage(storeBookImage(request.getBook().getImage(), saved));
            } catch (Exception ex) {
                ex.printStackTrace();
                throw new RuntimeException("Error storing book portrait image");
            }
        }

        return new AddBookResponse();
    }

    @Transactional(rollbackOn = Exception.class)
    public UpdateBookResponse updateBook(UpdateBookRequest request) {
        Book book = bookRepository.findById(request.getBook().getId())
                .orElseThrow(() -> new RuntimeException("Book not found"));
        Author author = authorRepository.findById(request.getBook().getAuthor().getId())
                .orElseThrow(() -> new RuntimeException("Author not found"));
        List<Genre> genres = new ArrayList<>();

        request.getBook().getGenres()
                .forEach(genreModel -> genres.add(genreRepository.findById(genreModel.getId())
                        .orElseThrow(() -> new RuntimeException("Genre with id " + genreModel.getId() + "not found"))));

        if (request.getBook().getTitle() != null) {
            book.setTitle(request.getBook().getTitle());
        }

        if (request.getBook().getYear() != 0) {
            book.setYear(request.getBook().getYear());
        }

        if (request.getBook().getIsbn() != null) {
            book.setIsbn(request.getBook().getIsbn());
        }

        if (request.getBook().getPublisher() != null) {
            book.setPublisher(request.getBook().getPublisher());
        }

        if (request.getBook().getPrice() != 0) {
            book.setPrice(request.getBook().getPrice());
        }

        book.setAuthor(author);
        book.setGenres(genres);

        if (request.getBook().getImage() != null) {
            try {
                deleteBookPortraitImage(book);
                book.setImage(storeBookImage(request.getBook().getImage(), book));
            } catch (Exception ex) {
                ex.printStackTrace();
                throw new RuntimeException("Error updating book portrait image");
            }
        }

        bookRepository.save(book);
        return new UpdateBookResponse();
    }

    public DeleteBookResponse deleteBook(DeleteBookRequest request) {
        Book book = bookRepository.findById(request.getId())
                .orElseThrow(() -> new RuntimeException("Book not found"));
        deleteBookPortraitImage(book);
        bookRepository.delete(book);
        return new DeleteBookResponse();
    }

    private String storeBookImage(RequestImage image, Book book) throws Exception {
        byte[] bytes = image.getData();
        Path folderPath = Paths.get("public/books-portraits");
        String filename = String.format("%s-%s", book.getId(), System.currentTimeMillis());
        String outputPath = String.format("%s/%s.jpg", folderPath.toAbsolutePath(), filename);
        InputStream is = new ByteArrayInputStream(bytes);
        BufferedImage bufferedImage = ImageIO.read(is);
        BufferedImage scaledImg = Scalr.resize(bufferedImage, Scalr.Method.QUALITY,
                200, 300, Scalr.OP_ANTIALIAS);

        if (!folderPath.toFile().exists() && !folderPath.toFile().mkdirs()) {
            throw new RuntimeException("Error creating folder");
        }

        float quality = 0.9f;
        ImageWriter writer = ImageIO.getImageWritersByFormatName("jpg").next();
        ImageWriteParam param = writer.getDefaultWriteParam();
        param.setCompressionMode(ImageWriteParam.MODE_EXPLICIT);
        param.setCompressionQuality(quality);
        writer.setOutput(new FileImageOutputStream(new File(outputPath)));
        writer.write(null, new IIOImage(scaledImg, null, null), param);

        return String.format("%s.jpg", filename);
    }

    private void deleteBookPortraitImage(Book book) {
        if (book.getImage() != null) {
            Path imagePath = Paths.get("public/books-portraits", book.getImage());

            if (!imagePath.toFile().exists()) {
                return;
            }

            try {
                Files.delete(imagePath);
            } catch (IOException ex) {
                ex.printStackTrace();
                throw new RuntimeException("Error deleting image");
            }
        }
    }

    private BookModel mapBookToBookModel(Book book) {
        BookModel bookModel = new BookModel();
        AuthorModel authorModel = new AuthorModel();
        Author author = book.getAuthor();
        XMLGregorianCalendar authorBirthdate = DateUtil.toXMLGregorianCalendar(author.getBirthdate());

        authorModel.setId(author.getId());
        authorModel.setFirstname(author.getFirstname());
        authorModel.setLastname(author.getLastname());
        authorModel.setPseudonym(author.getPseudonym());
        authorModel.setBirthdate(authorBirthdate);

        bookModel.setId(book.getId());
        bookModel.setTitle(book.getTitle());
        bookModel.setYear(book.getYear());
        bookModel.setIsbn(book.getIsbn());
        bookModel.setPrice(book.getPrice());
        bookModel.setAuthor(authorModel);

        if (book.getImage() != null) {
            bookModel.setImageUrl(String.format("http://localhost:8000/public/books-portraits/%s", book.getImage()));
        }

        book.getGenres().forEach(genre -> {
            GenreModel genreModel = new GenreModel();
            genreModel.setId(genre.getId());
            genreModel.setName(genre.getName());
            bookModel.getGenres().add(genreModel);
        });

        return bookModel;
    }
}
