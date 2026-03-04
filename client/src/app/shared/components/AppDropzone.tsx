import { UploadFile } from "@mui/icons-material";
import { FormControl, FormHelperText, Typography, useTheme } from "@mui/material";
import { useCallback } from "react";
import { useDropzone } from "react-dropzone";
import { useController, type UseControllerProps, type FieldValues } from "react-hook-form"

type Props<T extends FieldValues> = {
  name: keyof T
} & UseControllerProps<T>

export default function AppDropzone<T extends FieldValues>(props: Props<T>) {
  const {fieldState, field} = useController({...props});
  
  const theme = useTheme();

  const onDrop = useCallback((acceptedFiles: File[]) => {
    if (acceptedFiles.length > 0) {
      const fileWithPreview = Object.assign(acceptedFiles[0], {
        preview: URL.createObjectURL(acceptedFiles[0])
      });

      field.onChange(fileWithPreview);
    }
  },[field]);

  const {getRootProps, getInputProps, isDragActive} = useDropzone({onDrop});

  const dzStyles = {
    display:'flex',
    border:'dashed 2px #767676',
    borderColor: '#767676',
    borderRadius: '5px',
    paddingTop: '30px',
    alignItems:'center',
    height:200,
    width: 500
  }

  const dzActive = {
    borderColor: theme.palette.success.main
  }

  const dzSelected = {
    borderColor: theme.palette.success.main
  }

  return (
    <div {...getRootProps()}>
      <FormControl
        style={isDragActive ? {...dzStyles, ...dzActive} : field.value ? {...dzStyles, ...dzSelected} : dzStyles}
        error={!!fieldState.error}
      >
        <input {...getInputProps()} />
        <UploadFile sx={{fontSize: '100px'}} />
        <Typography variant="h4">Drop image here</Typography>
        <FormHelperText>{fieldState.error?.message}</FormHelperText>
      </FormControl>
    </div>
  )
}