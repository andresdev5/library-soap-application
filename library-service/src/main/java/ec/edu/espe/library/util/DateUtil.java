package ec.edu.espe.library.util;

import javax.xml.datatype.DatatypeFactory;
import java.time.LocalDate;

public class DateUtil {
    public static LocalDate toLocalDate(javax.xml.datatype.XMLGregorianCalendar xmlGregorianCalendar) {
        return xmlGregorianCalendar.toGregorianCalendar().toZonedDateTime().toLocalDate();
    }

    public static javax.xml.datatype.XMLGregorianCalendar toXMLGregorianCalendar(LocalDate date) {
        try {
            return DatatypeFactory.newInstance().newXMLGregorianCalendar(date.toString());
        } catch (Exception e) {
            return null;
        }
    }
}
