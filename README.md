# Files Bundler Cli

ה Files Bundler CLI הוא כלי שורת פקודה (CLI) שמאפשר לארוז מספר קבצי טקסט לקובץ אחד בקלות וביעילות. הכלי מציע אפשרויות רבות להתאמה אישית של אופן האריזה, כולל סידור הקבצים, הוספת הערות, סינון לפי שפות תכנות ועוד.




----
### דוגמת שימוש

**הגשת תרגילים למורה**:

כאשר נדרשת הגשת תרגילים למורה, אפשר להריץ את הפקודה הזו כדי לאחד את כל הקבצים לקובץ אחד, כדי להקל על ההעברה והבדיקה:

    fib bundle --output "submission.cs" --sort name --remove-empty-lines -n --author "Student_Name" --lang cs
### יצירת קובץ תגובה (Response File):

הכלי מאפשר גם יצירה של קובץ תגובה (`.rsp`), שכולל את כל האפשרויות באופן אוטומטי. קובץ זה שימושי כאשר רוצים לאחסן את כל ההגדרות של הפקודות מראש ולהריץ את הכלי בקלות.

כדי ליצור קובץ תגובה, יש להריץ את הפקודה הבאה: `fib create-rsp`

לאחר מכן, הכלי יבקש ממך להזין את הפרמטרים הדרושים (כמו שם הקובץ, האם להוסיף הערות, איך לסדר את הקבצים וכו'). קובץ התגובה `bundle.rsp` יווצר, וניתן להריץ אותו כך: `fib @bundle.rsp`



### הפלט של הפקודה `fib bundle --help`:
```bash
Options:
  -o, --output <output>                File path and name for the bundled output
  -n, --note                           Include the source file path as a comment
  -s, --sort <sort>                    Sort files by name (default) or extension [default: name]
  -r, --remove-empty-lines, --rml      Remove empty lines
  -a, --author <author>                Add the author's name as a header comment
  -l, --lang, --language <language>    Specify programming languages to include.
                                       Use 'all' to include all files (default) [default: all]
  -?, -h, --help                       Show help and usage information
```   
----
### טכנולוגיה:

הכלי נבנה על **.NET 8** ומשתמש ב-`System.CommandLine` לניהול הפקודות והאופציות בשורת הפקודה.
