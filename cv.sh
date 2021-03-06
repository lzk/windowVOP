v1=1
v2=0
v3=0
v4=1

if test $# -lt 1
then

printf "Usage: cv.sh [abcd]
Generate version information using version number \"a.b.c.d\". If the numbers
were missed. Default version \"%d.%d.%d.%d\" will be used.
Example: cv.sh 1003" $v1 $v2 $v3 $v4

else
    str=$1
    v1=${str:0:1}
    v2=${str:1:1}
    v3=${str:2:1}
    v4=${str:3:1}
fi

git log --format="#define GIT_HASH \"%H\"" -1 > VersionInfo.rc

printf "\
VS_VERSION_INFO VERSIONINFO\n\
 FILEVERSION %d,%d,%d,%d\n\
 PRODUCTVERSION %d,%d,%d,%d\n\
 FILEFLAGSMASK 0x3fL\n\
#ifdef _DEBUG\n\
 FILEFLAGS 0x1L\n\
#else\n\
 FILEFLAGS 0x0L\n\
#endif\n\
 FILEOS 0x40004L\n\
 FILETYPE 0x1L\n\
 FILESUBTYPE 0x0L\n\
BEGIN\n\
    BLOCK \"StringFileInfo\"\n\
    BEGIN\n\
        BLOCK \"040904B0\"\n\
        BEGIN\n\
            VALUE \"CompanyName\"      , \"Liteon\"\n\
            VALUE \"ProductName\"      , \"Faroe virtual panel\"\n\
            VALUE \"LegalCopyright\"   , \"(C) Liteon\"\n\
            VALUE \"FileDescription\"  , \"usbapi\"\n\
            VALUE \"InternalName\"     , \"usbapi.dll\"\n\
            VALUE \"OriginalFilename\" , \"usbapi.dll\"\n\
            VALUE \"PrivateBuild\"     , GIT_HASH\n\
            VALUE \"FileVersion\"      , \"%d.%d.%d.%d\"\n\
            VALUE \"ProductVersion\"   , \"%d.%d.%d.%d\"\n\
        END\n\
    END\n\
    BLOCK \"VarFileInfo\"\n\
    BEGIN\n\
        VALUE \"Translation\", 0x409, 1200\n\
    END\n\
END\n\
"  \
$v1 $v2 $v3 $v4 \
$v1 $v2 $v3 $v4 \
$v1 $v2 $v3 $v4 \
$v1 $v2 $v3 $v4 \
>> VersionInfo.rc

printf "\
using System.Reflection;\n\
using System.Runtime.InteropServices;\n\
using System.Windows;\n\
\n\
[assembly: AssemblyConfiguration(\"\")]\n\
[assembly: AssemblyProduct(\"Faroe virtual panel\")]\n\
" > VOP/Properties/VersionInfo.cs

git log --format="[assembly: AssemblyDescription(\"%H\")]" -1 >> VOP/Properties/VersionInfo.cs

printf "\
[assembly: AssemblyFileVersion(\"%d.%d.%d.%d\")]\n\
[assembly: AssemblyCompany(\"Liteon\")]\n\
[assembly: AssemblyCopyright(\"(C) Liteon\")]\n\
[assembly: ComVisible(false)]\n\
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]\n\
[assembly: AssemblyTitle(\"Faroe virtual panel\")]\n\
[assembly: AssemblyTrademark(\"\")]\n\
[assembly: AssemblyVersion(\"%d.%d.%d.%d\")]\n\
" \
$v1 $v2 $v3 $v4 \
$v1 $v2 $v3 $v4 \
>> VOP/Properties/VersionInfo.cs

printf "\
using System.Reflection;\n\
using System.Runtime.InteropServices;\n\
\n\
[assembly: AssemblyConfiguration(\"\")]\n\
[assembly: AssemblyProduct(\"Faroe virtual panel\")]\n\
" > VopHelper/Properties/VersionInfo.cs

git log --format="[assembly: AssemblyDescription(\"%H\")]" -1 >> VopHelper/Properties/VersionInfo.cs

printf "\
[assembly: AssemblyFileVersion(\"%d.%d.%d.%d\")]\n\
[assembly: AssemblyCompany(\"Liteon Co., Ltd.\")]\n\
[assembly: AssemblyCopyright(\"(C) Liteon Co., Ltd. (2015-2016)\")]\n\
[assembly: ComVisible(false)]\n\
[assembly: AssemblyTitle(\"Liteon virtual panel\")]\n\
[assembly: AssemblyTrademark(\"\")]\n\
[assembly: AssemblyVersion(\"%d.%d.%d.%d\")]\n\
" \
$v1 $v2 $v3 $v4 \
$v1 $v2 $v3 $v4 \
>> VopHelper/Properties/VersionInfo.cs

printf "\
using System.Reflection;\n\
using System.Runtime.InteropServices;\n\
using System.Runtime.CompilerServices;\n\
using System.Windows.Markup;\n\
\n\
[assembly: AssemblyConfiguration(\"\")]\n\
[assembly: AssemblyProduct(\"Lenovo virtual panel\")]\n\
" > WpfAnimatedGif/Properties/VersionInfo.cs

git log --format="[assembly: AssemblyDescription(\"%H\")]" -1 >> WpfAnimatedGif/Properties/VersionInfo.cs

printf "\
[assembly: AssemblyFileVersion(\"%d.%d.%d.%d\")]\n\
[assembly: AssemblyCompany(\"Lenovo Co., Ltd.\")]\n\
[assembly: AssemblyCopyright(\"(C) Lenovo Co., Ltd. (2015-2016)\")]\n\
[assembly: ComVisible(false)]\n\
[assembly: AssemblyTitle(\"Lenovo virtual panel\")]\n\
[assembly: AssemblyTrademark(\"\")]\n\
[assembly: AssemblyVersion(\"%d.%d.%d.%d\")]\n\
" \
$v1 $v2 $v3 $v4 \
$v1 $v2 $v3 $v4 \
>> WpfAnimatedGif/Properties/VersionInfo.cs

printf "\
using System.Reflection;\n\
using System.Runtime.InteropServices;\n\
\n\
[assembly: AssemblyConfiguration(\"\")]\n\
[assembly: AssemblyProduct(\"Lenovo virtual panel\")]\n\
" > CRMUploader/Properties/AssemblyInfo.cs

git log --format="[assembly: AssemblyDescription(\"%H\")]" -1 >> CRMUploader/Properties/AssemblyInfo.cs

printf "\
[assembly: AssemblyFileVersion(\"%d.%d.%d.%d\")]\n\
[assembly: AssemblyCompany(\"Lenovo Co., Ltd.\")]\n\
[assembly: AssemblyCopyright(\"(C) Lenovo Co., Ltd. (2015-2016)\")]\n\
[assembly: ComVisible(false)]\n\
[assembly: AssemblyTitle(\"CRMUploader\")]\n\
[assembly: AssemblyTrademark(\"\")]\n\
[assembly: AssemblyVersion(\"%d.%d.%d.%d\")]\n\
" \
$v1 $v2 $v3 $v4 \
$v1 $v2 $v3 $v4 \
>> CRMUploader/Properties/AssemblyInfo.cs

printf "\
using System.Reflection;\n\
using System.Runtime.InteropServices;\n\
\n\
[assembly: AssemblyConfiguration(\"\")]\n\
[assembly: AssemblyProduct(\"Lenovo virtual panel\")]\n\
" > SetPortAdmin/Properties/AssemblyInfo.cs

git log --format="[assembly: AssemblyDescription(\"%H\")]" -1 >> SetPortAdmin/Properties/AssemblyInfo.cs

printf "\
[assembly: AssemblyFileVersion(\"%d.%d.%d.%d\")]\n\
[assembly: AssemblyCompany(\"Lenovo Co., Ltd.\")]\n\
[assembly: AssemblyCopyright(\"(C) Lenovo Co., Ltd. (2015-2016)\")]\n\
[assembly: ComVisible(false)]\n\
[assembly: AssemblyTitle(\"SetPortAdmin\")]\n\
[assembly: AssemblyTrademark(\"\")]\n\
[assembly: AssemblyVersion(\"%d.%d.%d.%d\")]\n\
" \
$v1 $v2 $v3 $v4 \
$v1 $v2 $v3 $v4 \
>> SetPortAdmin/Properties/AssemblyInfo.cs