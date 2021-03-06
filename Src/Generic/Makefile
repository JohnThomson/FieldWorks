
#	$Id: Makefile 2650 2009-12-15 14:15:18Z beilharz $
#
#	Makefile for Generic library
#
#	Neil Mayhew - 29 Aug 2006

BUILD_ROOT = ../..
include $(BUILD_ROOT)/Bld/_names.mak
BUILD_PRODUCT = $(GENERIC_NAME)
include $(BUILD_ROOT)/Bld/_init.mak.lnx

OUTPUT  = $(FWOUTPUT)
INCLUDE = $(FWINCLUDE)

TLB_SRC      = $(OUTPUT)/Common

DEFINES  := $(DEFINES) -DGR_FW -DSUPPRESS_FW_EXCEPTION_HANDLING

ifeq ($(BUILD_CONFIG),Debug)
	DEBUG_LIBS = $(OUT_DIR)/libDebugProcs.a
	DEFINES  := $(DEFINES) -D_DEBUG
	OPTIMIZATIONS = -O0
else
	OPTIMIZATIONS = -O3
endif

PACKAGES := gtkmm-2.4

INCLUDES := $(shell icu-config --cppflags) \
	 $(INCLUDES) \
	-I$(GENERIC_SRC) -I$(TLB_SRC) -I$(INCLUDE) \
	-I$(WIN32MORE_INC) \
	-I$(COM_INC) \
	-I$(WIN32BASE_INC) \
	$(shell pkg-config --cflags $(PACKAGES)) \


LDLIBS = \
	-L$(WIN32MORE_LIB) -lWin32More \
	-L$(COM_LIB) -lcom \
	-L$(WIN32BASE_LIB) -lWin32Base \
	$(shell pkg-config --libs $(PACKAGES)) \
	$(shell icu-config --ldflags) \
	-ldl

LINK_LIBS := \
	$(LINK_LIBS) \
	$(DEBUG_LIBS) \

CPPFLAGS = $(DEFINES) $(INCLUDES) -MMD
CXXFLAGS = -g $(OPTIMIZATIONS) -fPIC -fvisibility=hidden -Werror -Waddress -Warray-bounds -Wmissing-field-initializers -Wclobbered -Wreturn-type -fstack-protector-all -Wshadow -rdynamic -std=c++11

ARFLAGS  = -crs

# === Object Lists ===

# We can't add TextProps1.o since it includes files from Kernel
OBJS = \
	$(INT_DIR)/ComVector.o \
	$(INT_DIR)/Debug.o \
	$(INT_DIR)/FileStrm.o \
	$(INT_DIR)/GenericFactory.o \
	$(INT_DIR)/HashMap.o \
	$(INT_DIR)/MakeDir.o \
	$(INT_DIR)/OleStringLiteral.o \
	$(INT_DIR)/StackDumper.o \
	$(INT_DIR)/StackDumperLinux.o \
	$(INT_DIR)/StringStrm.o \
	$(INT_DIR)/StringTable.o \
	$(INT_DIR)/StrUtil.o \
	$(INT_DIR)/TextProps.o \
	$(INT_DIR)/UnicodeConverter.o \
	$(INT_DIR)/UnicodeString8.o \
	$(INT_DIR)/Util.o \
	$(INT_DIR)/UtilFile.o \
	$(INT_DIR)/UtilSil.o \
	$(INT_DIR)/UtilString.o \
	$(INT_DIR)/UtilTime.o \
	$(INT_DIR)/UtilXml.o \
	$(INT_DIR)/FwSettings.o \

NOT_NEEDED = \
	DataStream.o \
	ResourceStrm.o \
	StrUtil.o

# === Targets ===

all: $(OUT_DIR)/libGeneric.a link_check

$(OUT_DIR)/libGeneric.a: $(OBJS)
	$(AR) $(ARFLAGS) $@ $^

link_check: $(INT_DIR)/libGeneric

$(INT_DIR)/libGeneric: $(OBJS) $(INT_DIR)/main.o $(INT_DIR)/ModuleEntry.o $(LINK_LIBS)
	$(LINK.cc) -o $@ $^ $(LDLIBS)

# === Rules ===

-include $(OBJS:%.o=%.d)
-include $(PCHNAME:%.gch=%.d)

clean:
	$(RM) $(OUT_DIR)/libGeneric.a $(INT_DIR)/libGeneric $(INT_DIR)/*.[od] $(INT_DIR)/*.gch
