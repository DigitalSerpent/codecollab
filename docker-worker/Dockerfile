FROM ubuntu:22.04

RUN apt-get update && apt-get install -y \
    python3 \
    nodejs \
    ruby \
    bash \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY runner.py .

CMD ["python3", "runner.py"]