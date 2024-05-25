package ec.edu.espe.library.endpoint;

import ec.edu.espe.library.service.AuthorService;
import ec.edu.espe.library.xjc.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.ws.server.endpoint.annotation.Endpoint;
import org.springframework.ws.server.endpoint.annotation.PayloadRoot;
import org.springframework.ws.server.endpoint.annotation.RequestPayload;
import org.springframework.ws.server.endpoint.annotation.ResponsePayload;

@Endpoint
public class AuthorsEndpoint {
    private static final String NAMESPACE_URI = "https://www.espe.edu.ec/library";
    private final AuthorService authorService;

    @Autowired
    public AuthorsEndpoint(AuthorService authorService) {
        this.authorService = authorService;
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "getAuthorsRequest")
    @ResponsePayload
    public GetAuthorsResponse getAuthors(@RequestPayload GetAuthorsRequest request) {
        return authorService.getAllAuthors();
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "getAuthorRequest")
    @ResponsePayload
    public GetAuthorResponse getAuthor(@RequestPayload GetAuthorRequest request) {
        return authorService.getAuthor(request.getId());
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "addAuthorRequest")
    @ResponsePayload
    public AddAuthorResponse addAuthor(@RequestPayload AddAuthorRequest request) {
        return authorService.addAuthor(request.getAuthor());
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "updateAuthorRequest")
    @ResponsePayload
    public UpdateAuthorResponse updateAuthor(@RequestPayload UpdateAuthorRequest request) {
        return authorService.updateAuthor(request);
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "deleteAuthorRequest")
    @ResponsePayload
    public DeleteAuthorResponse deleteAuthor(@RequestPayload DeleteAuthorRequest request) {
        return authorService.deleteAuthor(request);
    }
}
