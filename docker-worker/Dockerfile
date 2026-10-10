FROM ubuntu:22.04
RUN apt-get update && apt-get install -y python3 nodejs bash && rm -rf /var/lib/apt/lists/*
WORKDIR /app
RUN useradd -m sandbox
USER sandbox
CMD ["bash"]
