VERSION := v1.1.0
ZIP := $(VERSION).zip
SRC_DIR := release_extra

.PHONY: release FORCE
release: $(ZIP)

$(ZIP): FORCE
	@rm -f $(ZIP)
	@tmpdir=$$(mktemp -d); \
	mkdir -p $$tmpdir/PinItems && \
	cp -a $(SRC_DIR)/. $$tmpdir/PinItems/ && \
	(cd $$tmpdir && zip -r "$(CURDIR)/$(ZIP)" PinItems >/dev/null) && \
	rm -rf $$tmpdir

FORCE:
