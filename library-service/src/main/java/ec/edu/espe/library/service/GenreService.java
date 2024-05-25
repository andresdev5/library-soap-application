package ec.edu.espe.library.service;

import ec.edu.espe.library.entity.Genre;
import ec.edu.espe.library.repository.GenreRepository;
import ec.edu.espe.library.xjc.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class GenreService {
    private final GenreRepository genreRepository;

    @Autowired
    public GenreService(GenreRepository genreRepository) {
        this.genreRepository = genreRepository;
    }

    public GetGenresResponse getAllGenres() {
        GetGenresResponse response = new GetGenresResponse();

        genreRepository.findAll().forEach(genre -> {
            GenreModel model = new GenreModel();
            model.setId(genre.getId());
            model.setName(genre.getName());
            response.getGenres().add(model);
        });

        return response;
    }

    public GetGenreResponse getGenre(GetGenreRequest request) {
        GenreModel model = new GenreModel();
        long id = request.getId();
        model.setId(id);
        model.setName(genreRepository.findById(id).orElseThrow(() -> new RuntimeException("Genre not found")).getName());
        GetGenreResponse response = new GetGenreResponse();
        response.setGenre(model);
        return response;
    }

    public AddGenreResponse addGenre(AddGenreRequest request) {
        Genre genre = new Genre();
        genre.setName(request.getName());
        genreRepository.save(genre);

        return new AddGenreResponse();
    }

    public DeleteGenreResponse deleteGenre(DeleteGenreRequest request) {
        genreRepository.deleteById(request.getId());
        return new DeleteGenreResponse();
    }

    public UpdateGenreResponse updateGenre(UpdateGenreRequest request) {
        Genre genre = genreRepository.findById(request.getId()).orElseThrow(() -> new RuntimeException("Genre not found"));
        genre.setName(request.getName());
        genreRepository.save(genre);
        return new UpdateGenreResponse();
    }
}
