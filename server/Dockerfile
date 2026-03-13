# Build Stage
FROM maven:3.9.6-eclipse-temurin-17 AS builder
WORKDIR /app
COPY pom.xml .
RUN mvn dependency:go-offline -B
COPY src ./src
COPY conf ./conf
RUN mvn clean package -DskipTests

# Run Stage
FROM eclipse-temurin:17-jre
WORKDIR /app

# Copy the built fat jar with dependencies
COPY --from=builder /app/target/HonkaiServer-1.0-SNAPSHOT-jar-with-dependencies.jar ./server.jar
# Copy the config files
COPY --from=builder /app/conf ./conf

# Expose the server port
EXPOSE 1729

# Run the server
CMD ["java", "-cp", "server.jar", "com.zzhgl.app.core.GameServer"]
