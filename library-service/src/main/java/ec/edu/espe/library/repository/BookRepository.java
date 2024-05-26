package ec.edu.espe.library.repository;

import ec.edu.espe.library.entity.Book;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface BookRepository extends JpaRepository<Book, Long> {
    List<Book> findAllByOrderByIdDesc();
}
