package ec.edu.espe.library.config;

import org.springframework.boot.web.servlet.ServletRegistrationBean;
import org.springframework.context.ApplicationContext;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.io.ClassPathResource;
import org.springframework.ws.config.annotation.EnableWs;
import org.springframework.ws.config.annotation.WsConfigurerAdapter;
import org.springframework.ws.transport.http.MessageDispatcherServlet;
import org.springframework.ws.wsdl.wsdl11.DefaultWsdl11Definition;
import org.springframework.xml.validation.XmlValidator;
import org.springframework.xml.xsd.XsdSchema;
import org.springframework.xml.xsd.SimpleXsdSchema;
import org.springframework.xml.xsd.XsdSchemaCollection;

import java.util.logging.Logger;

@EnableWs
@Configuration
public class WebServiceConfig extends WsConfigurerAdapter {
    @Bean
    public ServletRegistrationBean<MessageDispatcherServlet> messageDispatcherServlet(ApplicationContext applicationContext) {
        MessageDispatcherServlet servlet = new MessageDispatcherServlet();
        servlet.setApplicationContext(applicationContext);
        servlet.setTransformWsdlLocations(true);
        return new ServletRegistrationBean<>(servlet, "/ws/*");
    }

    @Bean(name = "library")
    public DefaultWsdl11Definition defaultWsdl11Definition() {
        DefaultWsdl11Definition wsdl11Definition = new DefaultWsdl11Definition();
        wsdl11Definition.setPortTypeName("LibraryPort");
        wsdl11Definition.setLocationUri("/ws");
        wsdl11Definition.setTargetNamespace("https://www.espe.edu.ec/library");
        wsdl11Definition.setSchemaCollection(schemaCollection());
        return wsdl11Definition;
    }

    @Bean
    public XsdSchemaCollection schemaCollection() {
        return new XsdSchemaCollection() {
            @Override
            public XsdSchema[] getXsdSchemas() {
                SimpleXsdSchema[] schemas = new SimpleXsdSchema[] {
                    new SimpleXsdSchema(new ClassPathResource("xsd/authors.xsd")),
                    new SimpleXsdSchema(new ClassPathResource("xsd/genres.xsd")),
                    new SimpleXsdSchema(new ClassPathResource("xsd/books.xsd"))
                };

                for (SimpleXsdSchema schema : schemas) {
                    try {
                        schema.afterPropertiesSet();
                    } catch (Exception e) {
                        Logger.getLogger(WebServiceConfig.class.getName()).severe(e.getMessage());
                    }
                }

                return schemas;
            }

            @Override
            public XmlValidator createValidator() {
                throw new UnsupportedOperationException();
            }
        };
    }
}
