using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace LibCommon {

    #if DEBUG
    ///<summary>LogFormatter - trace messages</summary>
    public static class LF {

        [StringFormatMethod("format")] [MustUseReturnValue]
        public static string W(this string format, params object[] args) //
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        // ----------------------------------------------------------------

        public static string ToLiteral(this object src) {

            string from;
            try { from = src.ToString(); }
            catch { return "?"; }

            var sb = new StringBuilder();
            // Consider checking if provider creation is expensive
            using (var provider = CodeDomProvider.CreateProvider("CSharp")) {

                var chunkSize = 80;
                for (var i = 0; i < from.Length; i += chunkSize) {

                    if (i + chunkSize > from.Length) chunkSize = from.Length - i;
                    // Consider performance improvement
                    using (var writer = new StringWriter()) {
                        provider.GenerateCodeFromExpression(new System.CodeDom.CodePrimitiveExpression(
                            from.Substring(i, chunkSize)), writer, null);
                        sb.Append(writer.ToString().Trim('"'));
                    }
                }
            }
            return sb.ToString();
        }
        // ----------------------------------------------------------------

        ///<summary>Trace message formatter base class</summary>
        public abstract class Fmt {

            public abstract string Text { get; }
            public string NewlineText => "\n" + Text;

            public override string ToString() //
                { return Text; }

            public static implicit operator string(Fmt fmt) //
                { return fmt.ToString(); }
        }
        // ________________________________________________________________


        public class CFunction : Fmt {
            readonly string _detail;
            // ----------------------------------------------------------------
            public override string Text { get; }

            // ----------------------------------------------------------------

            [MethodImpl(MethodImplOptions.NoInlining)]
            public CFunction(string detail = null, params object[] args) {

                _detail = detail;

                var pass_args = new object[0];

                if (args != null) {
                    
                    if (args.Length > 0) {
                        
                        if (args[0] != null || args.Length == 1)
                            throw new InvalidOperationException(
                                "Argument list should start with null, following by at least one argument");
                    }
                    if (args.Length > 1)
                        pass_args = args.Skip(1).ToArray();
                }

                var stack_frame = new StackFrame(2, true);
                var func = stack_frame.GetMethod();

                var func_args = func?.GetParameters() ?? new ParameterInfo[0];
                var func_args_len = func_args.Length;

                var print_details = (func_args_len == pass_args.Length);
                if (print_details && !(func?.IsGenericMethod ?? false)) {

                    for (var i = 0; i < func_args_len; i++) {

                        //if (func_args[i].ParameterType.IsGenericParameter) {
                        //	continue;
                        //}
                        if (pass_args[i] == null) //
                            continue;

                        var func_arg_type = func_args[i].ParameterType;
                        var pass_arg_type = pass_args[i].GetType();

                        var pass = false;
                        if (func_arg_type.FullName.EndsWith("&")) {

                            if (string.Compare(func_arg_type.FullName, 0, pass_arg_type.FullName, 0,
                                    pass_arg_type.FullName.Length, true) == 0)
                                pass = true;
                        }

                        if (func_arg_type == typeof (object)) continue;

                        // if (! (func_arg_type == pass_arg_type || pass_arg_type.IsSubclassOf(func_args[i].ParameterType))
                        if (func_arg_type.IsAssignableFrom(pass_arg_type) || pass) continue;

                        print_details = false;
                        break;
                    }
                }
                // ----------------------------------------------------------------

                var type_name = "";
                if (func?.ReflectedType != null)
                    type_name = _g.TypeName(func.ReflectedType);

                var generic_parameters_literal = (func == null) ? "" : _g.GetGenericTypeR(func);

                var is_extension = false;
                var func_attr = func?.GetCustomAttributes(false) ?? new object[0];
                if (func_attr.Length > 0)
                    is_extension = func_attr.OfType<ExtensionAttribute>().Any();
                // ----------------------------------------------------------------

                var func_name = (func == null) ? "" : _g.GetFuncNameAnon(func);

                var fmt_args = new object[func_args.Length * 2 + 1];
                var sb_fmt = new StringBuilder();

                var info = "* {0}{1}{2}{3}({4}".W(
                        type_name,
                        (func?.IsStatic ?? true) ? "::" : ".",
                        func_name, // ctor workaround?
                        generic_parameters_literal,
                        is_extension ? "this " : "");

                var j = 0;
                if (print_details) {

                    for (var i = 0; i < func_args.Length; i++, j += 1) {

                        //sb_fmt.Append(((j == 0) ? "" : ", ") + "{" + j + "}={" + (j + 1) + "}");
                        //fmt_args[j] = func_args[i].Name;
                        //fmt_args[j + 1] = _g.GetPrintableValueR(pass_args[i]);

                        sb_fmt.Append(((j == 0) ? "" : ", ") + "{" + j + "}");

                        string tname, tvalue;
                        _g.GetPrintableValueR(pass_args[i], out tname, out tvalue);

                        if (tname.Length > 0 && tname.IndexOfAny(new[] { '(', ')' }) < 0)
                            tname += " ";

                        fmt_args[j] = "{0}{1}={2}".W(tname, func_args[i].Name, tvalue);
                    }
                }
                else {

                    // () no args
                    // ( ) unknown args
                    sb_fmt.Append(func_args.Length > 0 ? " " : "");
                }
                sb_fmt.Append(")");
                Text = info + sb_fmt.ToString().W(fmt_args)
                    + ((_detail == null) ? "" : ": ")
                    + _detail;
            }
            // ----------------------------------------------------------------
        }
        // ________________________________________________________________


        public class CEx : Fmt {
            // ----------------------------------------------------------------
            public override string Text { get; }

            // ----------------------------------------------------------------

            [MethodImpl(MethodImplOptions.NoInlining)]
            public CEx(Exception exception) {

                try {
                    exception = exception.GetBaseException();
                    //var trace_str = exception.StackTrace;
#if no_pdb_avail
                    int line = -1;

                    // no pdb
                    try {

                        if (trace_str.Contains("line")) {

                            var int_chars = new List<char>();
                            for (int i = trace_str.Length - 1; i >= 0; --i) {

                                var ch = trace_str[i];
                                if (ch == ' ' && int_chars.Count == 0)
                                    continue;

                                if (char.IsDigit(ch))
                                    int_chars.Add(ch);
                                else
                                    break;
                            }
                            int_chars.Reverse();

                            if (int_chars.Count > 0)
                                line = Convert.ToInt32(new string(int_chars.ToArray()));
                        }
                    } catch { }
                    // todo filename
                    var sline = (line >= 0) ? " @ line {0}".W(line) : string.Empty;
#endif
                    var st = new StackTrace(exception, true);
                    Text = Trace_(st, "Error: {0}".W(exception.Message));

                } catch (Exception ex) {

                    // ...
                    Debug.WriteLine(Trace_(ex.GetBaseException().Message));
                    if (exception != null)
                        Text = exception.ToString();
                }
            }
            // ----------------------------------------------------------------
        }
        // ________________________________________________________________


        public class CTrace : Fmt {

            string _text;
            string _detail;
            // ----------------------------------------------------------------
            public override string Text => _text;
            // ----------------------------------------------------------------

            [MethodImpl(MethodImplOptions.NoInlining)]
            public CTrace(StackTrace trace, string detail = null) {

                if (trace == null)
                    throw new InvalidOperationException(); // err.
                Init(detail, trace);
            }
            // ----------------------------------------------------------------

            [MethodImpl(MethodImplOptions.NoInlining)]
            public CTrace(string detail = null, int add_depth = 0) {

                var trace = new StackTrace(2 + add_depth, true);
                Init(detail, trace);
            }
            // ----------------------------------------------------------------

            void Init(string detail, StackTrace trace) {

                try {

                    _detail = detail;
                    //var collapsed = false;

                    var frame_count = trace.FrameCount;
                    var kvp_list = new List<KeyValuePair<string, string>>();
                    var external_lines = 0;

                    for (var i = 0; i < frame_count; ++i) {

                        var frame = trace.GetFrame(i);

                        //func.Module;
                        var src_info = "?";

                        var file_name = frame.GetFileName();
                        if (file_name != null) {

                            var file_line = frame.GetFileLineNumber();
                            // This would work if built on same platform
                            file_name = file_name.Substring(file_name.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                            src_info = file_name + ":" + file_line;
                            //collapsed = false;
                        }
                        else {

                            ++external_lines;
                            /*
                        if (external_lines >= 4) {

                            if (!collapsed) {
                                kvp_list.Add(new KeyValuePair<string, string>(" More external stack", "..."));
                                collapsed = true;
                            }
                            continue;
                        }*/
                        }

                        var func = frame.GetMethod();
                        var signature = _g.FuncSignature(func);

                        var returntype = string.Empty;
                        if (!(func.IsConstructor || func.Name == ".cctor")) {

                            var ret_type = ((MethodInfo) func).ReturnType;
                            returntype = _g.TypeName(ret_type) + " "; // output hack " "
                        }

                        //var prefix = (i > 0) ? "{0:00} ".W(frame_count - i) : "* ";
                        var prefix = (i > 0) ? " - ".W(i) : "* ";

                        // TraceLinePart
                        var tlp = "{0}{1}{2}({3})".W(prefix, returntype, signature.Key,
                            string.Join(", ", signature.Value.ToArray()));

                        #region * Should be done better than this *
                        /*
					var tlp = "{0}{1} {2}(".W(prefix, returntype, signature.Key);
					var tlp_len = tlp.Length;

					var fp_len = signature.Value.Count;
					for (int fp = 0; fp < fp_len; ++fp) {
						
						tlp += signature.Value[fp];
						if (fp < fp_len - 1)
							tlp += ", ";
						
						// ... (a
						// .....b
						if (tlp.Length > len_max && fp < fp_len - 1)
							tlp += "\n".PadRight(tlp_len + 1);
					}
					tlp += ")";

					// Todo clean logic
					var tlp_lines = tlp.Split('\n');
					var lines_max_len = tlp_lines.Select(l => l.Length).Max();
					tlp = string.Join("\n", tlp_lines.Select(t => t.PadRight(lines_max_len)).ToArray());

					var tlp_top_len = lines_max_len;

					if (tlp_top_len > top_len)
						top_len = tlp_top_len;
					*/
                        #endregion
                        kvp_list.Add(new KeyValuePair<string, string>(tlp, src_info));
                    }
                    var top_len = kvp_list.Count == 0 ? 0 : kvp_list.Select(l => l.Value.Length).Max();

                    /*
                var top_len_nl = kvp_list.Select(l => l.Key.IndexOf('\n')).Max();
                if (top_len < top_len_nl)
                    top_len = top_len_nl; */

                    // padding occurs after last line break
                    var det = string.IsNullOrEmpty(_detail) ? string.Empty : "* {0}\n".W(_detail);

                    _text = "-------------------------\n" + det + string.Join("\n",
                        kvp_list.Select(p => "{0} : {1}".W(p.Value.PadRight(top_len, ' '), p.Key))
                            .ToArray()) + "\n-------------------------";
                }
                catch {
                    //LF.Ex_(ex).LogWith();
                }
            }
            // ----------------------------------------------------------------
        }
        // ________________________________________________________________


        public static CTrace Trace => new CTrace();
        // ----------------------------------------------------------------

        public static CFunction Func => new CFunction();
        // ----------------------------------------------------------------

        // Function style

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CFunction Func_(string detail = null, params object[] args) //
            { return new CFunction(detail, args); }
        // ----------------------------------------------------------------

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CEx Ex_(Exception ex) // consider: ExceptionEventArgs overload
            { return new CEx(ex); }
        // ----------------------------------------------------------------

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CTrace Trace_(string detail = null, int add_depth = 0) //
            { return new CTrace(detail, add_depth); }
        // ----------------------------------------------------------------

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CTrace Trace_(StackTrace st, string detail = null) //
            { return new CTrace(st, detail); }
        // ----------------------------------------------------------------
    }
    // ________________________________________________________________

    public static partial class _g {

        #region * Trace helpers *

        ///<summary>Returns type name formatted for consistency with source code</summary>
        public static string TypeName(Type t) {

            //if (t.Attributes.HasFlag(TypeAttributes.NestedPublic)) ;
            /*
            var bools = new[] {
                t.IsNested,
                t.IsNestedAssembly,
                t.IsNestedFamANDAssem,
                t.IsNestedFamily,
                t.IsNestedFamORAssem,
                t.IsNestedPrivate,
                t.IsNestedPublic,
            };
            foreach(var b in bools) {
                
                if (!t.IsGenericTypeDefinition && b) {

                    // t.FullName.Parse()
                    // t.ReflectedType
                    return GetTypeAlias(t) + GetGenericTypeR(t);
                    break;
                }
            }
            */

            // closure, also anon type
            if (t.Name.IndexOfAny(new[] { '<', '>' }) >= 0 && t.Name.Contains("__")) {

                // λ
                var tn = t.ReflectedType;

                // recurse a bit
                if (tn != null && tn.Name.IndexOfAny(new[] { '<', '>' }) >= 0 && t.Name.Contains("__"))
                    return "" + GetTypeAlias(tn.ReflectedType) + GetGenericTypeR(t); // +

                return "" + GetTypeAlias(t.ReflectedType) + GetGenericTypeR(t); // +
            }

            // nullable
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                return GetGenericTypeR(t, false) + "?";

            // out parameter
            if (t.Name.EndsWith("&") && t.Name.Length > 1)
                return GetTypeNameAlias(t.Name.Substring(0, t.Name.Length - 1)) + GetGenericTypeR(t) + "&";

            return GetTypeAlias(t) + GetGenericTypeR(t);
        }
        // ----------------------------------------------------------------

        public static string TrimTypeName(Type t) //
            { return TrimTypeName(t.Name); }
        // ----------------------------------------------------------------

        public static string TrimTypeName(string s) {

            var idx = s.IndexOf('`');
            var tname = (idx >= 0) ? s.Substring(0, idx) : s;
            return tname;
        }
        // ----------------------------------------------------------------

        public static KeyValuePair<string, List<string>> FuncSignature(MethodBase mb) {

            var parms = mb.GetParameters();

            var tmp = parms.Select(parm => TypeName(parm.ParameterType)).ToList();
            // Consider config option to control output
            //tmp.Add("{0} {1}".W(TypeName(parm.ParameterType), parm.Name));

            //var parameters = "({0})".W(string.Join(", ", tmp.ToArray()));
            var generic_args = GetGenericTypeR(mb);

            // reflected type is null for 'lambda_expression'

            var func_name = GetFuncNameAnon(mb);

            return new KeyValuePair<string, List<string>>(
                //mb.Name + generic_args,
                ((mb.ReflectedType == null) ? "" : TypeName(mb.ReflectedType))
                    + ((mb.IsStatic) ? "::" : ".")
                    + func_name
                    + generic_args,
                tmp
            );
            //return mb.ReflectedType.Name + ((mb.IsStatic) ? "::" : ".") + mb.Name + generic_args + parameters;
        }
        // ----------------------------------------------------------------

        public static string GetFuncNameAnon(MethodBase func) {

            var func_name = func.Name.TrimStart('.');
            // cheap
            if (func_name.IndexOfAny(new[] { '<', '>' }) >= 0 && func_name.Contains("__")) {

                // λ
                //string line_num = "{0}".W(GetInstanceField(typeof(StackFrame), stack_frame, "iLineNumber"));
                func_name = func_name.Replace("<", "").Replace(">b__", ".λ_"); // +"@{0}".W(line_num);
            }
            return func_name;
        }
        // ----------------------------------------------------------------

        public static string GetGenericTypeR(Type x, bool decorate = true) {

            var generic_args = x.GetGenericArguments();
            return GetGenericTypeR(generic_args, decorate);
        }
        // ----------------------------------------------------------------

        public static string GetGenericTypeR(MethodBase mb, bool decorate = true) {

            if (mb == null)
                return "";

            if (mb.IsConstructor) // Generic args not supported in ctor
                return "";

            // generic types are null for 'lambda_expression'
            Type[] generic_args;
            try {
                if (mb.IsGenericMethod || mb.IsGenericMethodDefinition)
                    generic_args = mb.GetGenericArguments();
                else
                    return string.Empty;
            } catch { return string.Empty; }
            return GetGenericTypeR(generic_args, decorate);
        }
        // ----------------------------------------------------------------

        public static string GetGenericTypeR(Type[] gta, bool decorate = true) {

            if (gta.Length < 1)
                return string.Empty;

            var tmp = gta.Select(x => "{0}{1}".W(GetTypeAlias(x), GetGenericTypeR(x))).ToList();
            return decorate 
                ? "<{0}>".W(string.Join(", ", tmp.ToArray())) 
                : string.Join(", ", tmp.ToArray());
        }
        // ----------------------------------------------------------------

        public static string GetTypeAlias(Type t) {
            return Aliases.ContainsKey(t) ? Aliases[t] : TrimTypeName(t);
        }
        // ----------------------------------------------------------------

        public static string GetTypeNameAlias(string type_name) {
            return Aliases2.ContainsKey(type_name) ? Aliases2[type_name] : TrimTypeName(type_name);
        }
        // ----------------------------------------------------------------

        public static bool IsPrintableObject(object o) //
            { return (o.ToString() == o.GetType().ToString()); }
        // ----------------------------------------------------------------

        public static string GetPrintableValueR(object value, bool show_types = true) //
            { string a, b; return GetPrintableValueR(value, out a, out b, show_types); }
        // ----------------------------------------------------------------

        public static string GetPrintableValueR(object value, out string tname, out string tvalue, bool show_types = true) {

            // TODO limit recursion depth

            const int max_elems = 6;
            const int nmax = 64;

            //string printable_value;
            var sb = new StringBuilder();
            tname = string.Empty;

            if (value != null) {

                var value_type = value.GetType();

                // Rough check if ToString() returns something reasonable or a type name.
                // This would yield incorrect function argument value in an highly unlikely event
                // of object.ToString() returning a string matching it's internal type.
                //
                // var test = sys_helpers.custom();
                // if (test.ToString() != "sys_helpers.custom") { WRONG ACTION }

                // cheaper isPrintableObject?
                if (value.ToString() != value_type.ToString()) {

                    var type_name = TypeName(value_type).Trim(' ');

                    var value_literal = value.ToLiteral();
                    var vl_len = value_literal.Length;
                    
                    if (vl_len > nmax)
                        value_literal = value_literal.Substring(0, nmax - 8) + " ...(+{0} more)".W(vl_len - (nmax - 8));

                    var is_num = false;
                    if (Aliases.ContainsKey(value_type)) {

                        for (var i = 1; i < 12; ++i) { // byte to decimal

                            if (type_name != Aliases[value_type]) continue;

                            is_num = true;
                            break;
                        }
                    }

                    if (type_name.StartsWith("string"))
                        tvalue = "\"" + value_literal + "\"";
                    else if (is_num)
                        tvalue = value_literal;
                    else
                        tvalue = "{" + value_literal + "}";

                    if (show_types)
                        tname = "({0})".W(type_name);

                    sb.Append("{0}{1}".W(tname, tvalue));

                } else {
                    
                    var items = value as IEnumerable;
                    if (items != null) {

                        var i = 0;
                        var elems = new List<string>();

                        sb.Append("{ ");
                        foreach (var item in items) {

                            elems.Add(GetPrintableValueR(item, !(value_type.IsGenericType || value_type.IsArray)));
                            if (++i < max_elems) continue;
                            elems.Add("...");
                            break;
                        }
                        sb.Append(string.Join(", ", elems.ToArray()));
                        sb.Append(" }");

                        tname = TypeName(value_type);
                        tvalue = sb.ToString(); //printable_value;

                    } else { // default case, we do not know how to display type value(s)

                        tname = TypeName(value_type);
                        tvalue = "0x{0:x8}".W(value.GetHashCode()); // GetHexHashcode()

                        //printable_value = tname + tvalue;
                        sb.Append(tname);
                        sb.Append(tvalue);
                    }

                    /*else if (value_type.IsCOMObject) {

                        Type com_t = value.GetType();
                        printable_value = "{0}(\"{1}\")".W(trimTypeName(value_type_str), com_t.BaseType.GUID);

                    }*/
                }
            } else {

                tname = string.Empty; // can't detect type from null
                tvalue = "null"; // what if there's a 'null' string or something?

                sb.Append(tvalue);
            }

            return sb.ToString();
        }
        #endregion
        // ----------------------------------------------------------------

        #region * Type aliases *
        static readonly Dictionary<Type, string> Aliases = new Dictionary<Type, string>
        {

            { typeof(void),      "void"    },
            //
            { typeof(byte),      "byte"    }, // 1
            { typeof(sbyte),     "sbyte"   },
            { typeof(short),     "short"   },
            { typeof(ushort),    "ushort"  },
            { typeof(int),       "int"     },
            { typeof(uint),      "uint"    },
            { typeof(long),      "long"    },
            { typeof(ulong),     "ulong"   },
            { typeof(float),     "float"   },
            { typeof(double),    "double"  },
            { typeof(decimal),   "decimal" }, // 11
            { typeof(object),    "object"  },
            { typeof(bool),      "bool"    },
            { typeof(char),      "char"    },
            { typeof(string),    "string"  },
            //
            { typeof(byte[]),    "byte[]"    },
            { typeof(sbyte[]),   "sbyte[]"   },
            { typeof(short[]),   "short[]"   },
            { typeof(ushort[]),  "ushort[]"  },
            { typeof(int[]),     "int[]"     },
            { typeof(uint[]),    "uint[]"    },
            { typeof(long[]),    "long[]"    },
            { typeof(ulong[]),   "ulong[]"   },
            { typeof(float[]),   "float[]"   },
            { typeof(double[]),  "double[]"  },
            { typeof(decimal[]), "decimal[]" },
            { typeof(object[]),  "object[]"  },
            { typeof(bool[]),    "bool[]"    },
            { typeof(char[]),    "char[]"    },
            { typeof(string[]),  "string[]"  }

        };

        static readonly Dictionary<string, string> Aliases2 = new Dictionary<string, string>
        {

            { typeof(void).Name,      "void"    },
            //
            { typeof(byte).Name,      "byte"    }, // 1
            { typeof(sbyte).Name,     "sbyte"   },
            { typeof(short).Name,     "short"   },
            { typeof(ushort).Name,    "ushort"  },
            { typeof(int).Name,       "int"     },
            { typeof(uint).Name,      "uint"    },
            { typeof(long).Name,      "long"    },
            { typeof(ulong).Name,     "ulong"   },
            { typeof(float).Name,     "float"   },
            { typeof(double).Name,    "double"  },
            { typeof(decimal).Name,   "decimal" }, // 11
            { typeof(object).Name,    "object"  },
            { typeof(bool).Name,      "bool"    },
            { typeof(char).Name,      "char"    },
            { typeof(string).Name,    "string"  },
            //
            { typeof(byte[]).Name,    "byte[]"    },
            { typeof(sbyte[]).Name,   "sbyte[]"   },
            { typeof(short[]).Name,   "short[]"   },
            { typeof(ushort[]).Name,  "ushort[]"  },
            { typeof(int[]).Name,     "int[]"     },
            { typeof(uint[]).Name,    "uint[]"    },
            { typeof(long[]).Name,    "long[]"    },
            { typeof(ulong[]).Name,   "ulong[]"   },
            { typeof(float[]).Name,   "float[]"   },
            { typeof(double[]).Name,  "double[]"  },
            { typeof(decimal[]).Name, "decimal[]" },
            { typeof(object[]).Name,  "object[]"  },
            { typeof(bool[]).Name,    "bool[]"    },
            { typeof(char[]).Name,    "char[]"    },
            { typeof(string[]).Name,  "string[]"  }

        };

        #endregion
        // ----------------------------------------------------------------
    }
    // ________________________________________________________________
#else
    public static class LF {
        [StringFormatMethod("format")] [MustUseReturnValue]
        public static string W(this string format, params object[] args) //
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        public static string Trace => "";
        public static string Func => "";
        public static string Func_(string detail = null, params object[] args) => "";
    }
#endif
}
