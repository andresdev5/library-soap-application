package ec.edu.espe.library.endpoint;

import ec.edu.espe.library.service.GenreService;
import ec.edu.espe.library.xjc.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.ws.server.endpoint.annotation.Endpoint;
import org.springframework.ws.server.endpoint.annotation.PayloadRoot;
import org.springframework.ws.server.endpoint.annotation.RequestPayload;
import org.springframework.ws.server.endpoint.annotation.ResponsePayload;

@Endpoint
public class GenresEndpoint {
    private static final String NAMESPACE_URI = "https://www.espe.edu.ec/library";
    private final GenreService genreService;

    @Autowired
    public GenresEndpoint(GenreService genreService) {
        this.genreService = genreService;
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "getGenresRequest")
    @ResponsePayload
    public GetGenresResponse getGenres(@RequestPayload GetGenresRequest request) {
        return genreService.getAllGenres();
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "getGenreRequest")
    @ResponsePayload
    public GetGenreResponse getGenre(@RequestPayload GetGenreRequest request) {
        return genreService.getGenre(request);
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "addGenreRequest")
    @ResponsePayload
    public AddGenreResponse addGenre(@RequestPayload AddGenreRequest request) {
        return genreService.addGenre(request);
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "deleteGenreRequest")
    @ResponsePayload
    public DeleteGenreResponse deleteGenre(@RequestPayload DeleteGenreRequest request) {
        return genreService.deleteGenre(request);
    }

    @PayloadRoot(namespace = NAMESPACE_URI, localPart = "updateGenreRequest")
    @ResponsePayload
    public UpdateGenreResponse updateGenre(@RequestPayload UpdateGenreRequest request) {
        return genreService.updateGenre(request);
    }
}
