package ec.edu.espe.library.service;

import ec.edu.espe.library.entity.Author;
import ec.edu.espe.library.repository.AuthorRepository;
import ec.edu.espe.library.util.DateUtil;
import ec.edu.espe.library.xjc.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import javax.xml.datatype.XMLGregorianCalendar;
import java.time.LocalDate;
import java.util.List;

@Service
public class AuthorService {
    private final AuthorRepository authorRepository;

    @Autowired
    public AuthorService(AuthorRepository authorRepository) {
        this.authorRepository = authorRepository;
    }

    public GetAuthorsResponse getAllAuthors() {
        List<Author> authors = authorRepository.findAll();
        GetAuthorsResponse response = new GetAuthorsResponse();

        authors.forEach(author -> {
            response.getAuthor().add(mapAuthorToAuthorModel(author));
        });

        return response;
    }

    public GetAuthorResponse getAuthor(long id) {
        Author author = authorRepository.findById(id).orElseThrow(() -> new RuntimeException("Author not found"));
        GetAuthorResponse response = new GetAuthorResponse();
        response.setAuthor(mapAuthorToAuthorModel(author));
        return response;
    }

    public AddAuthorResponse addAuthor(AuthorModel authorModel) {
        Author author = new Author();
        LocalDate birthdate = authorModel.getBirthdate().toGregorianCalendar().toZonedDateTime().toLocalDate();
        author.setFirstname(authorModel.getFirstname());
        author.setLastname(authorModel.getLastname());
        author.setPseudonym(authorModel.getPseudonym());
        author.setBirthdate(birthdate);
        authorRepository.save(author);
        return new AddAuthorResponse();
    }

    public UpdateAuthorResponse updateAuthor(UpdateAuthorRequest request) {
        Author author = authorRepository.findById(request.getAuthor().getId()).orElseThrow(() -> new RuntimeException("Author not found"));
        LocalDate birthdate = DateUtil.toLocalDate(request.getAuthor().getBirthdate());

        if (request.getAuthor().getFirstname() != null) {
            author.setFirstname(request.getAuthor().getFirstname());
        }

        if (request.getAuthor().getLastname() != null) {
            author.setLastname(request.getAuthor().getLastname());
        }

        if (request.getAuthor().getPseudonym() != null) {
            author.setPseudonym(request.getAuthor().getPseudonym());
        }

        if (request.getAuthor().getBirthdate() != null) {
            author.setBirthdate(birthdate);
        }

        authorRepository.save(author);
        return new UpdateAuthorResponse();
    }

    public DeleteAuthorResponse deleteAuthor(DeleteAuthorRequest request) {
        Author author = authorRepository.findById(request.getId())
                .orElseThrow(() -> new RuntimeException("Author not found"));
        authorRepository.delete(author);
        return new DeleteAuthorResponse();
    }

    private AuthorModel mapAuthorToAuthorModel(Author author) {
        AuthorModel authorModel = new AuthorModel();
        XMLGregorianCalendar birthdate = DateUtil.toXMLGregorianCalendar(author.getBirthdate());

        authorModel.setId(author.getId());
        authorModel.setFirstname(author.getFirstname());
        authorModel.setLastname(author.getLastname());
        authorModel.setPseudonym(author.getPseudonym());
        authorModel.setBirthdate(birthdate);
        return authorModel;
    }
}
