# Use golden image as base image, cf. https://eng.ms/docs/more/containers-secure-supply-chain/
FROM mcr.microsoft.com/cbl-mariner/base/core:2.0

# Install R runtime and dependencies needed for R packages
RUN tdnf update -y && \
    tdnf -y install R-core R-core-devel which libsodium-devel ca-certificates tar build-essential glibc-iconv && \
    tdnf clean all

# Enable linking
RUN /usr/lib/rpm/mariner/gen-ld-script.sh mariner 2 /usr/src/mariner

# Install R packages and ensure that image build fails if any of the packages cannot be installed
RUN R -e "install.packages('remotes', repos='https://cloud.r-project.org')" # Needed to install packages with a specific version

RUN R -e "remotes::install_version('plumber', version = '1.2.1', repos = 'https://cloud.r-project.org'); if(!library(plumber, logical.return = T)) quit(status = 1)"
RUN R -e "remove.packages('swagger')" # Using Swagger introduces a vulnerability

RUN R -e "remotes::install_version('forecast', version = '8.21.1', repos = 'https://cloud.r-project.org'); if(!library(forecast, logical.return = T)) quit(status = 1)"
RUN R -e "remotes::install_version('plyr', version = '1.8.9', repos = 'https://cloud.r-project.org'); if(!library(plyr, logical.return = T)) quit(status = 1)"

RUN R -e "remotes::install_version('rpart.plot', version = '3.1.1', repos = 'https://cloud.r-project.org'); if(!library(rpart.plot, logical.return = T)) quit(status = 1)"
RUN R -e "remotes::install_version('rpart', version = '4.1.21', repos = 'https://cloud.r-project.org'); if(!library(rpart, logical.return = T)) quit(status = 1)"
RUN R -e "remotes::install_version('caret', version = '6.0.94', repos = 'https://cloud.r-project.org'); if(!library(caret, logical.return = T)) quit(status = 1)"
RUN R -e "remotes::install_version('RCurl', version = '1.98.1.14', repos = 'https://cloud.r-project.org'); if(!library(RCurl, logical.return = T)) quit(status = 1)"

COPY ./R/start_plumber.R /tmp/start_plumber.R
CMD ["Rscript", "/tmp/start_plumber.R"]
