CURRENT_DIR=$(shell pwd)

APP_NAME=Epicoin

SOURCE_DIR=/src/$(APP_NAME)/
BIN_DIR=/bin/


.PHONY: check update build clean clean-build

update:
	git pull

check:
	@type msbuild > /dev/null 2>&1 || (echo "msbuild not found. Please install msbuild"; exit 0;)
	@type mono > /dev/null 2>&1 || (echo "mono not found. Please install mono"; exit 0;)
	
clean: clean-build
	rm -rf $(CURRENT_DIR)$(BIN_DIR)*

clean-build:
	rm -rf $(CURRENT_DIR)$(SOURCE_DIR)bin/
	rm -rf $(CURRENT_DIR)$(SOURCE_DIR)obj/	

build: clean check
	mkdir -p $(CURRENT_DIR)$(BIN_DIR)
	@cd $(CURRENT_DIR)$(SOURCE_DIR) && msbuild /p:Configuration=Debug /v:m && msbuild /p:Configuration=Release /v:m 
	cd $(CURRENT_DIR)
	cp -r $(CURRENT_DIR)$(SOURCE_DIR)bin/* $(CURRENT_DIR)$(BIN_DIR)
	rm -rf $(CURRENT_DIR)$(BIN_DIR)Release/*.xml
	$(shell make clean-build)


