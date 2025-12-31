import os
import subprocess
import json

class ProjectBuilder:
    def __init__(self, config_path='ProjectConfigs.json', config_clone_path='ProjectConfigFormat.json'):
        
        if not os.path.exists(config_path):
            with open(config_clone_path, 'r', encoding='utf-8') as f:
                self.data = json.load(f)
        else:
            with open(config_path, 'r', encoding='utf-8') as f:
                self.data = json.load(f)
        
        # Thiết lập các biến chính (Context)
        self.ctx = self.data['settings']
        self.ctx['SOLUTION_NAME'] = f"{self.ctx['PROJECT_NAME']}Solution"
        self.ctx['PROJECT_DIR'] = f"C-Sharp--{self.ctx['PROJECT_TYPE']}"
        
        # Đường dẫn gốc (để đọc file txt sau khi chdir)
        self.root_path = os.getcwd()
        self.treePath = os.path.join(self.root_path, 'ProjectTree.txt')
        self.usagePath = os.path.join(self.root_path, 'ProjectUsage.txt')

        # Logic names (giống hệt code gốc của bạn)
        self.logic_names = {
            self.ctx['CORE']: f"{self.ctx['CORE']}Base",
            self.ctx['APP']: f"{self.ctx['APP']}Base",
            self.ctx['BASE']: f"{self.ctx['BASE']}Base",
            self.ctx['CLIENT']: f"{self.ctx['CLIENT']}Base"
        }

    def readFormatedTxt(self, path):
        if not os.path.exists(path): return ""
        with open(path, 'r', encoding='utf-8') as f:
            content = f.read()
        return content.format(**self.ctx)

    def run_command(self, command):
        print(f"Executing: {command}")
        process = subprocess.Popen(command, shell=True)
        process.wait()

    def rename_logic_files(self, proj_name, proj_type):
        """Khôi phục chính xác logic đổi tên của bạn."""
        path = f"src/{proj_name}"
        new_class_name = self.logic_names.get(proj_name, "BaseClass")
        
        if proj_type == "classlib":
            old_file = os.path.join(path, "Class1.cs")
            new_file = os.path.join(path, f"{new_class_name}.cs")
            if os.path.exists(old_file):
                with open(old_file, 'r', encoding='utf-8') as f:
                    content = f.read().replace("Class1", new_class_name)
                with open(new_file, 'w', encoding='utf-8') as f:
                    f.write(content)
                os.remove(old_file)

        elif proj_type == "winforms":
            base_files = {
                "Form1.cs": f"{new_class_name}.cs",
                "Form1.Designer.cs": f"{new_class_name}.Designer.cs",
                "Form1.resx": f"{new_class_name}.resx"
            }
            for old_name, new_name in base_files.items():
                old_path = os.path.join(path, old_name)
                new_path = os.path.join(path, new_name)
                if os.path.exists(old_path):
                    if old_name.endswith(".cs"):
                        with open(old_path, 'r', encoding='utf-8') as f:
                            content = f.read().replace("Form1", new_class_name)
                        with open(new_path, 'w', encoding='utf-8') as f:
                            f.write(content)
                        os.remove(old_path)
                    else:
                        os.rename(old_path, new_path)

            program_path = os.path.join(path, "Program.cs")
            if os.path.exists(program_path):
                with open(program_path, 'r', encoding='utf-8') as f:
                    content = f.read().replace("new Form1()", f"new {new_class_name}()")
                with open(program_path, 'w', encoding='utf-8') as f:
                    f.write(content)

    def create_project_structure(self):
        # Tạo thư mục gốc
        if not os.path.exists(self.ctx['PROJECT_DIR']):
            os.makedirs(self.ctx['PROJECT_DIR'])
        os.chdir(self.ctx['PROJECT_DIR'])
        os.makedirs(self.ctx['PROJECT_NAME'], exist_ok=True)
        os.chdir(self.ctx['PROJECT_NAME'])

        # dotnet new sln
        self.run_command(f"dotnet new sln -n {self.ctx['SOLUTION_NAME']}")

        # Directory.Build.props (từ JSON)
        with open("Directory.Build.props", "w", encoding="utf-8") as f:
            f.write(self.data['props'])

        # Tạo folder src/tests/build
        os.makedirs("src", exist_ok=True)
        os.makedirs("tests", exist_ok=True)
        os.makedirs("build", exist_ok=True) # Đảm bảo folder build tồn tại

        # Khởi tạo dự án con (Lấy từ JSON)
        projects = self.data['projects']
        for proj_alias, proj_template in projects.items():
            proj_real_name = self.ctx[proj_alias]
            path = f"src/{proj_real_name}"
            self.run_command(f"dotnet new {proj_template} -o {path}")
            self.run_command(f"dotnet sln add {path}/{proj_real_name}.csproj")
            self.rename_logic_files(proj_real_name, proj_template)

        # Thiết lập Project References (Dựa trên Alias trong context)
        APP, BASE, CLIENT, CORE = self.ctx['APP'], self.ctx['BASE'], self.ctx['CLIENT'], self.ctx['CORE']
        self.run_command(f"dotnet add src/{APP}/{APP}.csproj reference src/{CORE}/{CORE}.csproj")
        self.run_command(f"dotnet add src/{BASE}/{BASE}.csproj reference src/{APP}/{APP}.csproj")
        self.run_command(f"dotnet add src/{CLIENT}/{CLIENT}.csproj reference src/{BASE}/{BASE}.csproj")

        # Cài đặt NuGet Packages (Lấy từ JSON)
        packages_map = self.data['packages']
        for proj_alias, pkgs in packages_map.items():
            proj_real_name = self.ctx[proj_alias]
            for pkg in pkgs:
                self.run_command(f"dotnet add src/{proj_real_name}/{proj_real_name}.csproj package {pkg}")

        # Tạo thư mục con (Sub-folders)
        sub_folders = self.data['sub_folders']
        for proj_alias, folders in sub_folders.items():
            proj_real_name = self.ctx[proj_alias]
            for folder in folders:
                dir_path = f"src/{proj_real_name}/{folder}"
                os.makedirs(dir_path, exist_ok=True)
                with open(os.path.join(dir_path, ".gitkeep"), "w") as f: pass

        # Tạo Root Files (Lấy từ JSON)
        for filename, template in self.data['root_files_templates'].items():
            content = template.format(**self.ctx)
            with open(filename, "w", encoding="utf-8") as f:
                f.write(content)

        # Generate README
        tree = self.readFormatedTxt(self.treePath)
        usage = self.readFormatedTxt(self.usagePath)
        readme_content = f"# {self.ctx['PROJECT_NAME'].upper()}\n\n## I. STRUCTURE\n\n```text\n{tree}\n```\n\n## II. USAGE\n\n{usage}"
        with open("README.md", "w", encoding="utf-8") as f:
            f.write(readme_content)

        print(f"\n✅ COMPLETED: Project '{self.ctx['PROJECT_NAME']}' is ready.")

if __name__ == "__main__":
    builder = ProjectBuilder()
    builder.create_project_structure()